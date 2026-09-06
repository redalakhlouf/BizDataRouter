# Guide de Soutenance — Projet BIZOPSDATAROUTER

---

## 1. Architecture Complète du Projet

### Vue globale

```
BIZOPSDATAROUTER/
├── backend/          → API C# (.NET 10) — collecte des données industrielles
├── frontend/         → Interface React (Vite 8) — visualisation et contrôle
├── backend/envoi_api.py → Serveur mock qui simule une API industrielle (PI System)
└── backend/data/     → Fichiers CSV écrits localement avant upload
```

---

### BACKEND — Architecture

#### Le concept principal

Le backend est un **collecteur de données automatique**. Il interroge une API industrielle (PI System) chaque seconde, écrit les données en CSV, puis les upload sur MinIO (un stockage objet S3-compatible).

#### Fichiers et dossiers

| Fichier | Rôle |
|---------|------|
| `Program.cs` | **Point d'entrée** — configure tous les services, les routes, et démarre le pipeline |
| `appsettings.json` | **Configuration** — URLs, clés MinIO, taille max CSV |
| `Configuration/AppSettings.cs` | **Mapping** — convertit le JSON en objets C# typés |
| `Controllers/` | **Routes API** — les endpoints HTTP que le frontend appelle |
| `Models/` | **Structures de données** — DTOs, modèles PI, état du pipeline |
| `Services/` | **Logique métier** — lecture PI, écriture CSV, upload MinIO, orchestration |
| `data/` | **Fichiers CSV** — stockés localement avant transfert |

#### Les Services (le cœur du projet)

**1. `PipelineServices.cs` — La boucle principale**

```
Chaque seconde :
  → Appelle l'API PI (reçoit JSON)
  → Écrit les données en CSV (append)
  → Si le fichier est plein → upload sur MinIO
  → Attend 1 seconde
  → Recommence
```

**2. `PiApiReader.cs` — Appel à l'API industrielle**

- Envoie `GET http://localhost:3000/api/pidata/current`
- Reçoit un JSON avec des capteurs (pressure, temperature, etc.)
- Convertit le JSON en objets C# (`PiBatch`)

**3. `CsvFileSrevice.cs` — Écriture CSV**

- Gère le **nommage** : `TestDatarouterTemplate_year=2026_month=09_day=06_part=0.csv`
- Gère la **rotation** : quand un fichier atteint 50 MB, il crée `part=1`, `part=2`, etc.
- Gère le **changement de jour** : le prefix change, l'ancien fichier est uploadé

**4. `MinioStorageService.cs` — Upload MinIO**

- Vérifie que le bucket existe, le crée si nécessaire
- Upload le fichier CSV avec `PutObjectAsync`

**5. `PipelineCoordinator.cs` — Orchestrateur**

- Empêche deux pipelines de tourner en même temps (exclusion mutuelle avec `SemaphoreSlim`)
- Permet de démarrer/arrêter/redémarrer le pipeline via l'API

**6. `DataQueryService.cs` — Lecture des données**

- Quand le frontend demande `GET /api/data?date=2026-09-06`
- Ce service cherche les fichiers MinIO correspondant à cette date
- Les télécharge, les parse en CSV, et retourne les données

**7. `MinioQueryService.cs` — Liste des fichiers**

- Liste tous les objets dans le bucket MinIO
- Retourne nom, taille, date de chaque fichier

#### Les Routes API

| Méthode | Route | Ce qu'elle fait |
|---------|-------|-----------------|
| `GET` | `/api/health` | Vérifie que le serveur tourne |
| `GET` | `/api/status` | État du pipeline (running, connected, etc.) |
| `POST` | `/api/pipeline/start` | Démarre la collecte |
| `POST` | `/api/pipeline/stop` | Arrête la collecte |
| `POST` | `/api/pipeline/restart` | Redémarre |
| `GET` | `/api/config` | Configuration (sans secrets) |
| `GET` | `/api/data?date=...` | Données capteurs pour une date |
| `GET` | `/api/files` | Liste des fichiers MinIO |
| `GET` | `/api/files/{name}` | Infos d'un fichier |

#### Les Models

| Fichier | Rôle |
|---------|------|
| `PiModels.cs` | Structure du JSON PI : `PiBatch` → `PiElement` → `PiAttribute` |
| `PipelineStatus.cs` | **État mutable partagé** — isRunning, connected, lastUpload, etc. |
| `StatusDto.cs` | Réponse HTTP pour `/api/status` |
| `ConfigDto.cs` | Réponse HTTP pour `/api/config` (sans les secrets) |
| `PipelineCommandResponseDto.cs` | Réponse de start/stop/restart |
| `MinioFileDto.cs` | Représentation d'un fichier MinIO |
| `DataQueryDto.cs` | Données capteurs (SensorReading) + réponse journalière |

#### Le serveur mock (`envoi_api.py`)

- FastAPI sur le port 3000
- Simule 2 capteurs avec 4 attributs chacun (atelier, pressure, processtemp, randomvalues)
- Les valeurs de pressure/temp sont générées aléatoirement pour simuler des variations réelles

---

### FRONTEND — Architecture

#### Structure

| Fichier | Rôle |
|---------|------|
| `vite.config.js` | Proxy `/api/*` → `localhost:5000` |
| `src/services/api.js` | Tous les appels API (fetch wrapper) |
| `src/App.jsx` | Routeur + layout + polling status toutes les 4s |
| `src/pages/` | 5 pages : Dashboard, Pipeline, Data, Files, Configuration |
| `src/components/` | Composants réutilisables (Sidebar, Header, MetricCard, etc.) |

#### Pages

| Page | Route | Ce qu'elle fait |
|------|-------|-----------------|
| `DashboardPage` | `/` | Métriques, status, 5 dernières mesures |
| `PipelinePage` | `/pipeline` | Boutons Start/Stop/Restart + détails |
| `DataPage` | `/data` | Sélectionner une date → voir toutes les mesures |
| `FilesPage` | `/files` | Lister/filtrer les fichiers MinIO |
| `ConfigurationPage` | `/configuration` | Config backend (lecture seule) |

#### Communication avec le backend

- **HTTP/JSON** via `fetch()` natif
- **Polling** : status toutes les 4 secondes
- Pas de WebSocket, pas d'authentification

---

### Flux de données complet

```
1. API PI (envoi_api.py:3000) envoie des données capteurs en JSON
         ↓
2. PiApiReader les reçoit et convertit en objets C#
         ↓
3. CsvFileSrevice les écrit en CSV (append)
         ↓
4. MinioStorage les upload sur MinIO quand le fichier est plein
         ↓
5. Le frontend appelle GET /api/data?date=... pour lire les données
         ↓
6. DataQueryService les récupère depuis MinIO, les parse, et les retourne
```

---

## 2. Questions de Soutenance Probables

**Q : Pourquoi un pipeline en boucle au lieu d'un endpoint classique ?**
R : Les données industrielles sont continues (1 mesure/seconde). Le backend est l'acteur : il collecte en permanence, pas le client.

**Q : Comment tu gères les erreurs ?**
R : Le `try/catch` dans la boucle attrape les erreurs de connexion PI/MinIO, les note dans `PipelineStatus`, et continue après 1 seconde. Résilience par retry.

**Q : Pourquoi SemaphoreSlim dans le Coordinator ?**
R : Exclusion mutuelle. Sans ça, deux appels HTTP simultanés à `/start` créeraient deux boucles parallèles qui écriraient dans les mêmes fichiers.

**Q : Comment les fichiers sont nommés et pourquoi ?**
R : `Template_year=YYYY_month=MM_day=DD_part=N.csv`. Le nom encode la date → permet de filtrer sans télécharger le contenu.

**Q : Pourquoi les secrets MinIO ne sont pas dans ConfigDto ?**
R : Sécurité. On renvoie juste `AccessKeyConfigured: true/false` pour dire qu'elles sont configurées.

**Q : C'est quoi MinIO ?**
R : MinIO est un serveur de stockage objet compatible S3 (Amazon Simple Storage Service). C'est une alternative open-source à S3. On l'utilise pour stocker les fichiers CSV de manière centralisée et accessible.

**Q : C'est quoi un DTO ?**
R : DTO signifie Data Transfer Object. C'est un objet qui transporte des données entre les couches de l'application (Service → Controller → HTTP Response). On les utilise pour ne jamais exposer les objets internes directement.

**Q : Pourquoi utiliser IHostedService pour le PipelineCoordinator ?**
R : IHostedService est un mécanisme .NET qui lance un service automatiquement au démarrage de l'application. Le pipeline démarre tout seul sans qu'un client ait besoin d'appeler un endpoint.

**Q : Comment le CSV est géré ?**
R : On utilise la bibliothèque CsvHelper. L'écriture se fait en mode append (ajout). L'en-tête n'est écrit qu'une fois au début du fichier. La rotation se fait quand un fichier atteint 50 MB.

**Q : Qu'est-ce que le pattern de nommage Hive-style ?**
R : C'est un pattern de nommage basé sur des paires clé=valeur : `_year=2026_month=09_day=06`. C'est utilisé dans les data lakes (comme Hive, Hudi, Delta Lake) pour permettre le partitionnement et le filtrage efficace sans scanner tout le contenu.

**Q : Pourquoi avoir choisi .NET 10 ?**
R : .NET 10 est la dernière version LTS (Long Term Support) de Microsoft. Elle offre des performances améliorées, un support à long terme, et les dernières fonctionnalités du framework.

**Q : Comment le frontend communique avec le backend ?**
R : Le frontend envoie des requêtes HTTP/JSON au backend via fetch(). En développement, Vite proxyse les requêtes `/api/*` vers `localhost:5000`. Le status est pollé toutes les 4 secondes pour mettre à jour l'interface en temps réel.

**Q : Pourquoi pas de WebSocket ?**
R : Le polling toutes les 4 secondes est suffisant pour ce cas d'usage. Le status ne change pas en temps réel (le pipeline tourne ou non). Un WebSocket ajouterait de la complexité sans bénéfice significatif.

**Q : Qu'est-ce que l'injection de dépendances (DI) ?**
R : C'est un pattern où les objets reçoivent leurs dépendances de l'extérieur plutôt que de les créer eux-mêmes. Par exemple, `DataQueryService` reçoit `IMinioClient` dans son constructeur. .NET crée et injecte automatiquement cette instance. Cela rend le code plus testable et maintenable.
