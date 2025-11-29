# 🚀 Guide de démarrage rapide - FileSyncHub

Ce guide vous aidera à démarrer rapidement avec FileSyncHub.

## 📋 Prérequis

- ✅ .NET 9.0 SDK installé
- ✅ Docker et Docker Compose installés et en cours d'exécution
- ✅ (Optionnel) AWS CLI pour tester LocalStack

## 🏃 Démarrage en 3 étapes

### Étape 1 : Démarrer les services

Choisissez l'une des options suivantes :

#### Option A : Script automatique (recommandé)
```bash
./scripts/start-dev.sh
```

#### Option B : Makefile
```bash
make dev
```

#### Option C : Docker Compose
```bash
cp .env.example .env
docker compose up -d
```

### Étape 2 : Créer et appliquer les migrations

```bash
# Installer dotnet-ef si nécessaire
dotnet tool install --global dotnet-ef

# Créer la migration initiale
dotnet ef migrations add InitialCreate \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web

# Appliquer les migrations
dotnet ef database update \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web
```

Ou utilisez le Makefile :
```bash
make migration  # Pour créer une nouvelle migration
make migrate    # Pour appliquer les migrations
```

### Étape 3 : Lancer l'API

```bash
dotnet run --project src/Dotland.FileSyncHub.Web
```

Ou avec hot reload :
```bash
make watch
```

## ✅ Vérification

L'API devrait être accessible sur : **http://localhost:5000**

Testez avec :
```bash
curl http://localhost:5000/health
```

Réponse attendue :
```json
{
  "status": "healthy",
  "service": "ged-backend"
}
```

## 🔌 Informations de connexion

### PostgreSQL
- **Host:** localhost
- **Port:** 5432
- **Database:** filesyncdb
- **Username:** filesync
- **Password:** filesync_password

**Connection String :**
```
Host=localhost;Port=5432;Database=filesyncdb;Username=filesync;Password=filesync_password
```

### Redis
- **Host:** localhost
- **Port:** 6379
- **Password:** redis_password

**Connection String :**
```
localhost:6379,password=redis_password
```

## 🛠️ Outils de gestion (optionnel)

### Démarrer avec les outils UI

```bash
make dev-tools
# ou
docker compose --profile tools up -d
```

**Accès :**
- **pgAdmin :** http://localhost:5050
  - Email : admin@filesync.local
  - Password : admin

- **Redis Commander :** http://localhost:8081

## 🧪 Tester l'upload de fichiers

### 1. Configurer AWS S3 (Production) ou LocalStack (Dev)

**Pour LocalStack (développement local) :**

```bash
# Démarrer l'environnement complet avec LocalStack
make dev-full
# ou
./scripts/start-dev-full.sh

# Créer le bucket S3
docker exec filesync-localstack awslocal s3 mb s3://filesync-dev

# Vérifier
docker exec filesync-localstack awslocal s3 ls
```

**Pour AWS S3 (production) :**

Configurer les variables d'environnement :
```bash
export AWS_ACCESS_KEY_ID=your_key
export AWS_SECRET_ACCESS_KEY=your_secret
export S3__BucketName=your-bucket
export S3__Region=eu-west-1
```

### 2. Uploader un fichier de test

```bash
curl -X POST http://localhost:5000/api/v1/documents/upload \
  -F "file=@test.pdf" \
  -F "organizationId=org-123" \
  -F "category=General" \
  -F "description=Test document"
```

## 🔍 Commandes utiles

```bash
# Voir les logs des services
make logs
# ou
docker compose logs -f

# Vérifier l'état des services
make health
# ou
docker compose ps

# Arrêter les services
make down
# ou
./scripts/stop-dev.sh

# Nettoyer complètement (⚠️ supprime les données)
make clean
```

## 📚 Prochaines étapes

1. ✅ Explorez l'API avec les endpoints disponibles
2. ✅ Configurez le versioning pour votre organisation via `/api/versioning`
3. ✅ Testez l'upload de documents avec différentes catégories
4. ✅ Explorez les versions de documents
5. ✅ Consultez la documentation complète dans [README.md](README.md)

## ❓ Problèmes courants

### Le port 5432 est déjà utilisé
```bash
# Trouver le processus utilisant le port
sudo lsof -i :5432
# ou
sudo netstat -tulpn | grep 5432

# Arrêter PostgreSQL local si installé
sudo systemctl stop postgresql
```

### Erreur de connexion à la base de données
1. Vérifier que les services Docker sont démarrés : `docker compose ps`
2. Vérifier les logs : `docker compose logs postgres`
3. Tester la connexion : `docker exec filesync-postgres pg_isready -U filesync`

### L'API ne démarre pas
1. Vérifier que .NET 9.0 SDK est installé : `dotnet --version`
2. Restaurer les packages : `dotnet restore`
3. Nettoyer et rebuilder : `dotnet clean && dotnet build`

## 🎯 Aide

Pour plus d'informations :
- Consultez le [README.md](README.md) complet
- Voir les commandes disponibles : `make help`
- Documentation détaillée dans [docs/](docs/)
