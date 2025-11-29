# FileSyncHub

Système de gestion documentaire (GED) avec stockage AWS S3, construit avec .NET 9.0 et suivant les principes de Clean Architecture.

## 🏗️ Architecture

La solution suit une architecture en couches (Clean Architecture) avec 4 projets :

- **Domain** : Entités métier, énumérations, interfaces de repositories
- **Application** : Services applicatifs, logique métier, DTOs
- **Infrastructure** : Implémentation de la persistance (EF Core, PostgreSQL, Redis)
- **Web** : API REST ASP.NET Core, contrôleurs, services S3

## 🚀 Démarrage rapide

### Prérequis

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) et Docker Compose
- [AWS CLI](https://aws.amazon.com/cli/) (optionnel, pour LocalStack)

### 1. Démarrer les services (PostgreSQL + Redis)

**Option A : Utiliser les scripts Shell (recommandé)**

```bash
# Démarrer uniquement les bases de données
./scripts/start-dev.sh

# OU démarrer l'environnement complet (API + DB + LocalStack)
./scripts/start-dev-full.sh

# Arrêter tous les services
./scripts/stop-dev.sh
```

**Option B : Utiliser Docker Compose directement**

```bash
# Copier le fichier d'environnement
cp .env.example .env

# Démarrer PostgreSQL et Redis
docker compose up -d

# Démarrer avec les outils de gestion (pgAdmin + Redis Commander)
docker compose --profile tools up -d

# Vérifier l'état des services
docker compose ps
```

**Option C : Utiliser Makefile**

```bash
# Démarrer les bases de données
make dev

# Démarrer avec les outils de gestion
make dev-tools

# Démarrer l'environnement complet
make dev-full

# Arrêter les services
make down

# Voir toutes les commandes disponibles
make help
```

**Services disponibles** :
- PostgreSQL : `localhost:5432`
- Redis : `localhost:6379`
- pgAdmin (avec profile tools) : `http://localhost:5050`
- Redis Commander (avec profile tools) : `http://localhost:8081`
- API (avec docker-compose.dev.yml) : `http://localhost:5000`
- LocalStack S3 (avec docker-compose.dev.yml) : `localhost:4566`

### 2. Configurer l'application

Modifier `src/Dotland.FileSyncHub.Web/appsettings.Development.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=filesyncdb;Username=filesync;Password=filesync_password"
  },
  "UseInMemoryDatabase": false,
  "Redis": {
    "ConnectionString": "localhost:6379,password=redis_password"
  }
}
```

### 3. Créer et appliquer les migrations

```bash
# Installer l'outil EF Core (si pas déjà fait)
dotnet tool install --global dotnet-ef

# Créer la migration initiale
dotnet ef migrations add InitialCreate --project src/Dotland.FileSyncHub.Infrastructure --startup-project src/Dotland.FileSyncHub.Web

# Appliquer les migrations
dotnet ef database update --project src/Dotland.FileSyncHub.Infrastructure --startup-project src/Dotland.FileSyncHub.Web
```

### 4. Lancer l'application

```bash
# Restaurer les dépendances
dotnet restore

# Compiler
dotnet build

# Lancer l'API
dotnet run --project src/Dotland.FileSyncHub.Web

# Ou avec hot reload
dotnet watch --project src/Dotland.FileSyncHub.Web
```

L'API sera disponible sur : `http://localhost:5000`

## 📦 Gestion Docker Compose

### Commandes de base

```bash
# Démarrer tous les services
docker compose up -d

# Démarrer avec les outils de gestion UI
docker compose --profile tools up -d

# Arrêter les services
docker compose down

# Arrêter et supprimer les volumes (⚠️ supprime les données)
docker compose down -v

# Voir les logs
docker compose logs -f

# Voir les logs d'un service spécifique
docker compose logs -f postgres
docker compose logs -f redis

# Redémarrer un service
docker compose restart postgres
```

### Environnement complet avec API

Le fichier `docker-compose.dev.yml` inclut l'API et LocalStack pour un environnement complet :

```bash
# Démarrer l'environnement complet
docker compose -f docker-compose.dev.yml up -d

# Initialiser le bucket S3 dans LocalStack
docker exec filesync-localstack awslocal s3 mb s3://filesync-dev
docker exec filesync-localstack awslocal s3 ls

# Voir les logs de l'API
docker compose -f docker-compose.dev.yml logs -f api

# Arrêter
docker compose -f docker-compose.dev.yml down
```

## 🗄️ Structure des données

Les données sont persistées dans le dossier `data/` :
- `data/postgres/` : Données PostgreSQL
- `data/redis/` : Données Redis
- `data/pgadmin/` : Configuration pgAdmin

⚠️ Le dossier `data/` est exclu du contrôle de version (.gitignore)

## 🔧 Configuration

### Variables d'environnement (.env)

Voir `.env.example` pour toutes les variables disponibles.

### Configuration AWS S3

```bash
# Variables d'environnement pour AWS
export AWS_ACCESS_KEY_ID=your_access_key
export AWS_SECRET_ACCESS_KEY=your_secret_key
export S3__BucketName=your-bucket-name
export S3__Region=eu-west-1
```

### LocalStack (pour développement local S3)

```bash
# Démarrer LocalStack
docker run -d -p 4566:4566 localstack/localstack

# Créer un bucket
aws --endpoint-url=http://localhost:4566 s3 mb s3://filesync-dev

# Configurer l'application pour utiliser LocalStack
export S3__ServiceUrl=http://localhost:4566
```

## 🧪 Tests

```bash
# Lancer tous les tests
dotnet test

# Avec couverture de code
dotnet test --collect:"XPlat Code Coverage"
```

## 📚 Documentation

- [Documentation API](docs/ged-backend/README.md)
- [S3 Storage Service](docs/ged-backend/s3-storage-service.md)
- [Organisation S3](docs/ged-backend/s3-bucket-organization.md)

## 🛠️ Développement

### Structure du projet

```
file-sync-hub/
├── src/
│   ├── Dotland.FileSyncHub.Domain/       # Entités, énumérations, interfaces
│   ├── Dotland.FileSyncHub.Application/  # Services, DTOs, logique métier
│   ├── Dotland.FileSyncHub.Infrastructure/ # EF Core, repositories
│   └── Dotland.FileSyncHub.Web/          # API REST, contrôleurs
├── docs/                                  # Documentation
├── data/                                  # Données Docker (gitignore)
├── docker-compose.yml                     # Services Docker
└── .env.example                          # Variables d'environnement

```

### Commandes utiles

```bash
# Nettoyer les artefacts de build
dotnet clean

# Restaurer les packages NuGet
dotnet restore

# Formater le code
dotnet format

# Analyser le code
dotnet build /p:TreatWarningsAsErrors=true
```

## 📄 Licence

Copyright © Dotland 2024

## 🤝 Contribution

Les contributions sont les bienvenues ! Veuillez créer une issue avant de soumettre une pull request.
