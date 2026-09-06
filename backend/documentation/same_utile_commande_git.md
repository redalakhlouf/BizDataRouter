# SAME COMMANDE IN GIT THAT I LEARN IN THIS PROJET #

git push
git add
git commit -m ""
git log --oneline
git checkout idee de commit que tu veux
git branch : tu connais tous les branches de ton projet maintenat avec le quelle vert et avancer par *
# Pense-bête Git : Branches et Commandes de Base

## 📋 Lister les branches
```bash
git branch
```
*Affiche les branches locales. La branche active est en vert avec une étoile `*`.*

```bash
git branch -r
```
*Liste uniquement les branches distantes (sur GitHub/GitLab).*

```bash
git branch -a
```
*Liste toutes les branches (locales et distantes).*

```bash
git branch -v
```
*Affiche les branches avec le dernier commit associé.*

## 🆕 Créer et basculer
```bash
git branch <nom>
```
*Crée une nouvelle branche sans basculer dessus.*

```bash
git switch <nom>
```
*Permet de vous déplacer sur la branche spécifiée.*

```bash
git switch -c <nom>
```
*Crée la branche et bascule dessus en une seule étape.*

## ✂️ Supprimer des branches
```bash
git branch -d <nom>
```
*Supprime proprement une branche locale (uniquement si elle est fusionnée).*

```bash
git branch -D <nom>
```
*Force la suppression d'une branche locale (même non fusionnée).*

## ✏️ Renommer une branche
```bash
git branch -m <nouveau-nom>
```
*Renomme la branche sur laquelle vous vous trouvez actuellement.*

```bash
git branch -m <ancien-nom> <nouveau-nom>
```
*Renomme n'importe quelle branche locale.*

## 🚀 Flux de travail classique (Sauvegarde & Historique)
```bash
git add .
```
*Ajoute toutes les modifications au panier (staging area).*

```bash
git commit -m "feat: ajouter la gestion des branches"
```
*Enregistre les modifications avec un message clair.*

```bash
git push origin <nom-de-la-branche>
```
*Envoie vos commits locaux sur le dépôt distant (GitHub/GitLab).*

## 🔍 Historique et Navigation
```bash
git log --oneline
```
*Affiche l'historique des commits de manière très compacte (ID et message).*

```bash
git checkout <id-du-commit>
```
*Permet de voyager dans le temps et de regarder le projet à l'état de ce commit.*
# Guide Récapitulatif des Commandes

## 🛠️ Gestion des Fichiers et Dossiers
* `ls` : Lister les fichiers du répertoire courant
* `cd <dossier>` : Changer de répertoire
* `pwd` : Afficher le chemin du répertoire actuel
* `mkdir <nom>` : Créer un nouveau dossier
* `rm <fichier>` : Supprimer un fichier
* `rm -rf <dossier>` : Supprimer un dossier et son contenu

## 💻 Système et Réseau
* `top` : Afficher les processus en temps réel
* `df -h` : Vérifier l'espace disque disponible
* `ifconfig` : Afficher la configuration réseau (IP)
* `ping <hote>` : Tester la connexion vers un serveur

## 📦 Gestion des Paquets (Ubuntu/Debian)
* `sudo apt update` : Mettre à jour la liste des paquets
* `sudo apt upgrade` : Mettre à jour les logiciels installés
* `sudo apt install <nom>` : Installer un nouveau logiciel

## 📝 Consultation de Contenu
* `cat <fichier>` : Afficher le contenu complet d'un fichier
* `less <fichier>` : Naviguer dans un fichier page par page
* `tail -f <fichier>` : Suivre l'affichage d'un fichier en temps réel

# Changer de branche
git switch nom-de-la-branche

# Créer ET passer sur une nouvelle branche
git switch -c nouvelle-branche

# Changer de branche (comme switch)
git checkout nom-de-la-branche

# Créer ET passer sur une nouvelle branche
git checkout -b nouvelle-branche

# Annuler les modifications d'un fichier (action totalement différente !)
git checkout -- nom-du-fichier.txt


# uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
`app.main:app = dossier app, fichier main.py, objet app = FastAPI()`
`--reload = redémarre auto à chaque modif (dev uniquement, jamais en prod)`
`--host 0.0.0.0 = accessible depuis l'extérieur du conteneur, pas juste localhost`
`--port 8000 = port d'écoute`

# Pourquoi Docker plutôt que le .exe direct ?
* Le .exe marche, mais il tourne "nu" sur ta machine, dépend de ton OS, difficile à répliquer sur le VM de prod
 Docker encapsule MinIO dans un conteneur isolé, identique partout (ton PC, le VM du stage) — c'est la cohérence d'environnement  dont tu as besoin pour un vrai déploiement
 La commande docker run
 bash
# docker run -d ^
# --name minio-server ^
#  -p 9000:9000 -p 9001:9001 ^
#  -v C:\Users\HP\Desktop\minio\minio-data:/data ^
#  -e "MINIO_ROOT_USER=admin" ^
#  -e "MINIO_ROOT_PASSWORD=change_this_password" ^
#  minio/minio server /data --console-address ":9001" ou simplment 
docker run -d --name minio-server -p 9000:9000 -p 9001:9001 -v C:\Users\HP\Desktop\minio\minio-data:/data -e "MINIO_ROOT_USER=admin" -e "MINIO_ROOT_PASSWORD=admin" minio/minio server /data --console-address ":9001"

`(Le ^ est le retour à la ligne dans PowerShell/CMD Windows — si tu préfères tout sur une seule ligne, enlève les ^ et colle tout à la suite.)`

# Décomposition, ligne par ligne
`-d → détaché, le conteneur tourne en arrière-plan, tu récupères ton terminal
--name minio-server → nom du conteneur, pour pouvoir faire docker stop minio-server plus tard
-p 9000:9000 → port de l'API S3 (c'est celui que ton code C# et Power BI vont utiliser)
-p 9001:9001 → port de la console web (interface graphique pour voir tes buckets dans le navigateur)
-v C:\Users\HP\Desktop\minio\minio-data:/data → le plus important : ça relie ton dossier minio-data (que tu as déjà, visible dans ta capture) au dossier interne du conteneur. Si le conteneur est supprimé, tes données restent sur ton disque
-e "MINIO_ROOT_USER=..." / -e "MINIO_ROOT_PASSWORD=..." → tes identifiants d'accès (à changer, ne garde pas admin/mot de passe faible même en local)
minio/minio server /data --console-address ":9001" → l'image Docker officielle MinIO, on lui dit de servir depuis /data et d'exposer la console sur le port 9001 `
# curl http://localhost:9000/minio/health/live

# docker logs minio-server
` Cette commande te montre tout ce que MinIO a écrit dans sa sortie — utile pour voir l'erreur exacte s'il a planté.`

# git checkout main
` faire deplace toi sur le branch main `
# git branch -a
` -a → affiche toutes les branches, locales et distantes (celles qui existent sur le serveur mais que tu n'as pas encore récupérées localement)`

# git remote -v
` donne le lien ou le repo est utilise `# Récupérer la branche principale du repo BizOps
 
## Cas 1 — La branche s'appelle `main` sur le serveur distant, mais tu ne l'as pas en local
 
```bash
git checkout -b main origin/main
```
 
- `checkout -b main` : crée une nouvelle branche locale `main`
- `origin/main` : base cette branche sur celle qui existe déjà côté serveur (récupère tout l'historique)
## Cas 2 — La branche principale s'appelle `dev`
 
```bash
git checkout dev
git pull origin dev
```
 
- `git checkout dev` : bascule sur la branche `dev` (doit déjà exister en local)
- `git pull origin dev` : récupère (`fetch`) + fusionne (`merge`) les derniers commits du serveur dans ta copie locale
## Comment savoir lequel utiliser
 
```bash
git branch -a
```
 
Affiche toutes les branches (locales + distantes). Regarde le nom de la branche principale dans la liste avant de choisir Cas 1 ou Cas 2.
 
## Une fois sur la bonne branche principale — créer ta branche de travail
 
```bash
git checkout -b feature/nom-descriptif
```
 
Puis pour pousser tes changements :
 
```bash
git add .
git status
git commit -m "Message clair décrivant le changement"
git push -u origin feature/nom-descriptif
```
 

Option A — déplacer tout le contenu d'un dossier vers un autre

powershell
Move-Item -Path "C:\chemin\vers\ancien-dossier\*" -Destination "C:\Users\HP\Desktop\BizDataRouterToMinio" -Force
Move-Item → commande PowerShell pour déplacer
* à la fin du chemin source → tout le contenu du dossier (fichiers et sous-dossiers), pas le dossier lui-même
-Force → écrase si un fichier de même nom existe déjà à destination (attention avec ça, vérifie qu'il n'y a pas de conflit important avant)

Option B — copier plutôt que déplacer (plus prudent, garde une sauvegarde)

powershell
Copy-Item -Path "C:\chemin\vers\ancien-dossier\*" -Destination "C:\Users\HP\Desktop\BizDataRouterToMinio" -Recurse -Force
Copy-Item au lieu de Move-Item → laisse les originaux intacts dans l'ancien dossier, au cas où
-Recurse → nécessaire pour copier aussi les sous-dossiers, pas juste les fichiers à la racine
