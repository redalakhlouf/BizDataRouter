# API BizDataRouter

## Démarrage

Depuis la racine du projet :

```powershell
dotnet run
```

La console indique l'adresse sur laquelle l'API écoute, par exemple `http://localhost:5000`. Remplace `<URL_API>` dans les exemples par cette adresse.

Le pipeline démarre automatiquement avec l'application. S'il ne peut pas joindre PI Web API, l'API reste disponible : `GET /api/status` affichera l'erreur au lieu d'arrêter le serveur.

## Carte des endpoints

| Méthode | URL | Utilité |
| --- | --- | --- |
| `GET` | `/api/health` | Vérifie que l'API répond. |
| `GET` | `/api/status` | Affiche l'état réel du pipeline. |
| `GET` | `/api/config` | Affiche la configuration publique. |
| `GET` | `/api/files` | Liste les fichiers stockés dans MinIO. |
| `GET` | `/api/files/{fileName}` | Donne le détail d'un fichier MinIO. |
| `GET` | `/api/data?date=YYYY-MM-DD` | Lit les mesures CSV d'une date depuis MinIO. |
| `POST` | `/api/pipeline/start` | Démarre le pipeline s'il est arrêté. |
| `POST` | `/api/pipeline/stop` | Arrête le pipeline proprement. |
| `POST` | `/api/pipeline/restart` | Redémarre le pipeline. |

## 1. Vérifier que le serveur répond

```powershell
Invoke-RestMethod '<URL_API>/api/health'
```

Résultat attendu :

```json
{
  "status": "healthy",
  "timestamp": "2026-08-12T...Z"
}
```

## 2. Lire le statut du pipeline

```powershell
Invoke-RestMethod '<URL_API>/api/status'
```

Exemple de résultat :

```json
{
  "isRunning": true,
  "piApiConnected": true,
  "minioConnected": true,
  "lastApiCall": "2026-08-12T10:30:00Z",
  "activeFilePath": "C:\\...\\data\\Template_year=2026_month=08_day=12_part=0.csv",
  "lastUploadedFile": "Template_year=2026_month=08_day=12_part=0.csv",
  "lastUploadTime": "2026-08-12T10:31:00Z",
  "lastError": null,
  "lastErrorTime": null,
  "localFileCount": 1
}
```

Cet endpoint alimente les cartes principales du dashboard : état du pipeline, connexion PI, connexion MinIO, dernier upload et dernière erreur.

## 3. Lire la configuration publique

```powershell
Invoke-RestMethod '<URL_API>/api/config'
```

La clé secrète MinIO n'est volontairement jamais renvoyée : une API ne doit pas exposer ses identifiants au navigateur.

## 4. Lister les fichiers MinIO

```powershell
Invoke-RestMethod '<URL_API>/api/files'
```

Exemple :

```json
[
  {
    "name": "Template_year=2026_month=08_day=12_part=0.csv",
    "sizeBytes": 4182,
    "lastModified": "2026-08-12T10:31:00Z"
  }
]
```

## 5. Lire les mesures d'une journée

```powershell
Invoke-RestMethod '<URL_API>/api/data?date=2026-07-14'
```

Le résultat contient le nombre de fichiers trouvés, le nombre total de lignes et les mesures CSV. Si la journée n'existe pas dans MinIO, l'API retourne `404`.

## 6. Contrôler le pipeline

```powershell
Invoke-RestMethod '<URL_API>/api/pipeline/stop' -Method Post
```

```powershell
Invoke-RestMethod '<URL_API>/api/pipeline/start' -Method Post
```

```powershell
Invoke-RestMethod '<URL_API>/api/pipeline/restart' -Method Post
```

Chaque commande retourne le nouveau statut. Dans la future interface, les boutons **Start**, **Stop** et **Restart** appelleront exactement ces trois URLs.
