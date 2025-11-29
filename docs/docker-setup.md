# 🐳 Configuration Docker - FileSyncHub

Ce document explique la configuration Docker pour FileSyncHub.

## 📁 Fichiers de configuration

### docker-compose.yml
Configuration de base pour les services de développement :
- **PostgreSQL 16** : Base de données principale
- **Redis 7** : Cache et stockage de sessions
- **pgAdmin** (profile `tools`) : Interface de gestion PostgreSQL
- **Redis Commander** (profile `tools`) : Interface de gestion Redis

**Utilisation :**
```bash
docker compose up -d                    # Démarrer PostgreSQL + Redis
docker compose --profile tools up -d    # Inclure les outils de gestion
docker compose down                     # Arrêter tous les services
```

### docker-compose.dev.yml
Configuration complète pour le développement incluant :
- Tous les services de `docker-compose.yml`
- **LocalStack** : Émulation AWS S3 locale
- **API FileSyncHub** : Application .NET conteneurisée

**Utilisation :**
```bash
docker compose -f docker-compose.dev.yml up -d    # Environnement complet
docker compose -f docker-compose.dev.yml down     # Arrêter
```

### Dockerfile
Image Docker multi-stage pour l'API :
- **Build stage** : Compilation de l'application .NET 9.0
- **Runtime stage** : Image Alpine légère pour l'exécution
- **Sécurité** : Exécution en tant qu'utilisateur non-root
- **Health check** : Vérification de l'état via `/health`

**Build :**
```bash
docker build -t filesync-api:latest .
```

### .dockerignore
Exclusions pour la construction de l'image Docker :
- Fichiers de build (`bin/`, `obj/`)
- Données locales (`data/`)
- Fichiers de configuration (`*.env`)
- Documentation

## 🗄️ Persistance des données

Les données sont stockées localement dans le dossier `data/` :

```
data/
├── postgres/           # Données PostgreSQL
├── redis/             # Données Redis (AOF)
├── pgadmin/           # Configuration pgAdmin
└── localstack/        # Données LocalStack S3
```

**⚠️ Important :** Le dossier `data/` est exclu du contrôle de version (.gitignore)

## 🔐 Variables d'environnement

### Fichier .env

Copier `.env.example` vers `.env` et personnaliser :

```bash
cp .env.example .env
```

### Variables principales

#### PostgreSQL
```bash
POSTGRES_DB=filesyncdb              # Nom de la base de données
POSTGRES_USER=filesync              # Utilisateur
POSTGRES_PASSWORD=filesync_password # Mot de passe
POSTGRES_PORT=5432                  # Port exposé
```

#### Redis
```bash
REDIS_PASSWORD=redis_password       # Mot de passe Redis
REDIS_PORT=6379                     # Port exposé
REDIS_MAX_MEMORY=256mb              # Limite de mémoire
```

#### Outils de gestion
```bash
PGADMIN_EMAIL=admin@filesync.local  # Email pgAdmin
PGADMIN_PASSWORD=admin              # Mot de passe pgAdmin
PGADMIN_PORT=5050                   # Port pgAdmin

REDIS_COMMANDER_PORT=8081           # Port Redis Commander
```

#### Application
```bash
API_PORT=5000                       # Port de l'API
S3_BUCKET_NAME=filesync-dev         # Nom du bucket S3
AWS_ACCESS_KEY_ID=test              # Clé d'accès AWS (LocalStack)
AWS_SECRET_ACCESS_KEY=test          # Clé secrète AWS (LocalStack)
```

## 🏗️ Architecture réseau

### Network : filesync-network
Bridge network permettant la communication entre les conteneurs.

**Résolution DNS interne :**
- `postgres:5432` - PostgreSQL
- `redis:6379` - Redis
- `localstack:4566` - LocalStack S3
- `api:5000` - API FileSyncHub

## 🔍 Services détaillés

### PostgreSQL

**Image :** `postgres:16-alpine`

**Configuration :**
- Base de données : `filesyncdb`
- Utilisateur : `filesync`
- Données persistées dans : `./data/postgres`
- Health check : `pg_isready`

**Connexion depuis l'hôte :**
```bash
psql -h localhost -p 5432 -U filesync -d filesyncdb
# Mot de passe : filesync_password
```

**Connexion depuis Docker :**
```bash
docker exec -it filesync-postgres psql -U filesync -d filesyncdb
```

### Redis

**Image :** `redis:7-alpine`

**Configuration :**
- Mode AOF (Append Only File) activé
- Persistence : `appendfsync everysec`
- Limite mémoire : 256MB (configurable)
- Politique d'éviction : `allkeys-lru`

**Connexion depuis l'hôte :**
```bash
redis-cli -h localhost -p 6379 -a redis_password
```

**Connexion depuis Docker :**
```bash
docker exec -it filesync-redis redis-cli -a redis_password
```

### LocalStack (S3)

**Image :** `localstack/localstack:latest`

**Services activés :** S3 uniquement

**Utilisation :**
```bash
# Créer un bucket
docker exec filesync-localstack awslocal s3 mb s3://filesync-dev

# Lister les buckets
docker exec filesync-localstack awslocal s3 ls

# Lister les objets
docker exec filesync-localstack awslocal s3 ls s3://filesync-dev

# Uploader un fichier
docker exec filesync-localstack awslocal s3 cp test.txt s3://filesync-dev/
```

**Configuration dans l'application :**
```json
{
  "S3": {
    "ServiceUrl": "http://localstack:4566",
    "BucketName": "filesync-dev",
    "Region": "eu-west-1"
  }
}
```

### API FileSyncHub

**Build depuis :** `Dockerfile`

**Points de montage :**
- Code source (lecture seule) : `./src:/app/src:ro`

**Variables d'environnement injectées :**
- Connection strings pour PostgreSQL et Redis
- Configuration S3 pour LocalStack
- Mode développement

**Healthcheck :**
```bash
curl http://localhost:5000/health
```

### pgAdmin

**Image :** `dpage/pgadmin4:latest`

**Accès :** http://localhost:5050

**Configuration :**
- Email : `admin@filesync.local`
- Mot de passe : `admin`
- Mode standalone (pas de master password)

**Ajouter un serveur PostgreSQL :**
1. Clic droit sur "Servers" → "Register" → "Server"
2. General tab : Nom = "FileSyncHub"
3. Connection tab :
   - Host : `postgres`
   - Port : `5432`
   - Database : `filesyncdb`
   - Username : `filesync`
   - Password : `filesync_password`

### Redis Commander

**Image :** `rediscommander/redis-commander:latest`

**Accès :** http://localhost:8081

**Configuration automatique :**
- Host : `redis`
- Port : `6379`
- Password : `redis_password`

## 🚀 Scénarios d'utilisation

### 1. Développement local (sans Docker pour l'API)

```bash
# Démarrer uniquement les bases de données
docker compose up -d

# Lancer l'API en local
dotnet run --project src/Dotland.FileSyncHub.Web
```

**Avantages :**
- Hot reload avec `dotnet watch`
- Débogage dans l'IDE
- Modifications de code instantanées

### 2. Développement avec outils UI

```bash
# Démarrer avec pgAdmin et Redis Commander
docker compose --profile tools up -d

# Lancer l'API en local
dotnet run --project src/Dotland.FileSyncHub.Web
```

**Avantages :**
- Inspection visuelle des données PostgreSQL
- Monitoring Redis en temps réel

### 3. Environnement complet conteneurisé

```bash
# Tout dans Docker
docker compose -f docker-compose.dev.yml up -d

# Initialiser S3
docker exec filesync-localstack awslocal s3 mb s3://filesync-dev
```

**Avantages :**
- Environnement isolé
- Reproductible
- Proche de la production
- Inclut LocalStack pour S3

## 🧹 Maintenance

### Nettoyer les données

```bash
# Arrêter et supprimer les volumes
docker compose down -v

# Supprimer le dossier data
rm -rf data/
```

### Nettoyer les images

```bash
# Supprimer l'image de l'API
docker rmi filesync-api:latest

# Nettoyer les images non utilisées
docker image prune -a
```

### Voir l'utilisation des ressources

```bash
# État des conteneurs
docker compose ps

# Utilisation CPU/Mémoire
docker stats

# Espace disque
docker system df
```

## 🔧 Dépannage

### PostgreSQL ne démarre pas

```bash
# Vérifier les logs
docker compose logs postgres

# Problème de permissions sur data/
sudo chown -R $USER:$USER data/postgres

# Supprimer et recréer
docker compose down -v
docker compose up -d
```

### Redis ne démarre pas

```bash
# Vérifier les logs
docker compose logs redis

# Tester la connexion
docker exec filesync-redis redis-cli -a redis_password ping
```

### L'API ne peut pas se connecter à PostgreSQL

**Vérifier :**
1. Network : `docker network inspect filesync-network`
2. PostgreSQL est accessible : `docker exec filesync-postgres pg_isready`
3. Connection string utilise le nom du service : `Host=postgres` (pas `localhost`)

### LocalStack S3 ne répond pas

```bash
# Vérifier le health check
docker compose -f docker-compose.dev.yml ps

# Tester LocalStack
curl http://localhost:4566/_localstack/health

# Recréer le bucket
docker exec filesync-localstack awslocal s3 rb s3://filesync-dev --force
docker exec filesync-localstack awslocal s3 mb s3://filesync-dev
```

## 📚 Références

- [Docker Compose documentation](https://docs.docker.com/compose/)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres)
- [Redis Docker Hub](https://hub.docker.com/_/redis)
- [LocalStack documentation](https://docs.localstack.cloud/)
- [.NET Docker images](https://hub.docker.com/_/microsoft-dotnet)
