# Changelog - Configuration Docker

## [2024-11-29] - Ajout de l'infrastructure Docker complète

### ✨ Nouveautés

#### Configuration Docker Compose

**`docker-compose.yml`** - Configuration de base pour le développement
- ✅ PostgreSQL 16 Alpine avec persistance locale
- ✅ Redis 7 Alpine avec AOF (Append Only File)
- ✅ pgAdmin 4 pour la gestion visuelle de PostgreSQL (profile `tools`)
- ✅ Redis Commander pour la gestion visuelle de Redis (profile `tools`)
- ✅ Network bridge dédié : `filesync-network`
- ✅ Health checks pour tous les services
- ✅ Configuration via variables d'environnement

**`docker-compose.dev.yml`** - Environnement de développement complet
- ✅ Tous les services de `docker-compose.yml`
- ✅ LocalStack pour émulation AWS S3 locale
- ✅ Application FileSyncHub API conteneurisée
- ✅ Configuration automatique des connexions entre services
- ✅ Hot reload du code source (volume monté)

#### Image Docker

**`Dockerfile`** - Build optimisé multi-stage
- ✅ Base : .NET 9.0 SDK Alpine (build) + ASP.NET 9.0 Alpine (runtime)
- ✅ Build incrémental avec cache des layers NuGet
- ✅ Sécurité : utilisateur non-root (`appuser`)
- ✅ Health check intégré sur `/health`
- ✅ Taille optimisée avec Alpine Linux

**`.dockerignore`**
- ✅ Exclusion des artefacts de build
- ✅ Exclusion des données locales
- ✅ Optimisation de la taille du contexte Docker

#### Scripts d'automatisation

**`scripts/start-dev.sh`**
- ✅ Démarrage automatique PostgreSQL + Redis
- ✅ Création du fichier `.env` s'il n'existe pas
- ✅ Vérification de la santé des services
- ✅ Affichage des informations de connexion

**`scripts/start-dev-full.sh`**
- ✅ Démarrage de l'environnement complet (DB + Redis + LocalStack + API)
- ✅ Instructions pour initialiser S3

**`scripts/stop-dev.sh`**
- ✅ Arrêt propre de tous les services

#### Makefile

**`Makefile`** - 15+ commandes pour simplifier le développement
- `make dev` - Démarrer PostgreSQL et Redis
- `make dev-tools` - Démarrer avec pgAdmin et Redis Commander
- `make dev-full` - Démarrer l'environnement complet
- `make migrate` - Exécuter les migrations EF Core
- `make migration` - Créer une nouvelle migration
- `make build` - Compiler la solution
- `make run` / `make watch` - Lancer l'API localement
- `make test` - Exécuter les tests
- `make health` - Vérifier l'état des services
- `make clean` - Nettoyer complètement (⚠️ supprime les données)
- `make help` - Afficher toutes les commandes

#### Configuration

**`.env.example`** - Template de configuration
- ✅ Variables PostgreSQL (DB, user, password, port)
- ✅ Variables Redis (password, port, max memory)
- ✅ Variables pgAdmin (email, password, port)
- ✅ Variables Redis Commander (port)
- ✅ Variables API (port, S3 bucket, AWS credentials)

#### Documentation

**`README.md`** (mis à jour)
- ✅ Section complète sur Docker Compose
- ✅ 3 options de démarrage (scripts, Makefile, Docker Compose)
- ✅ Guide de configuration de l'application
- ✅ Instructions pour les migrations
- ✅ Exemples de commandes

**`QUICKSTART.md`** (nouveau)
- ✅ Guide de démarrage en 3 étapes
- ✅ Informations de connexion
- ✅ Commandes de test
- ✅ Troubleshooting des problèmes courants

**`docs/docker-setup.md`** (nouveau)
- ✅ Documentation détaillée de la configuration Docker
- ✅ Explication de chaque service
- ✅ Architecture réseau
- ✅ Scénarios d'utilisation (3 modes de développement)
- ✅ Guide de dépannage avancé
- ✅ Commandes d'administration

#### Gestion des données

**`.gitignore`** (mis à jour)
- ✅ Exclusion du dossier `data/` (volumes Docker)
- ✅ Exclusion des fichiers de base de données SQLite (*.db, *.db-shm, *.db-wal)

### 🗄️ Structure de persistance

```
data/                   # Créé automatiquement par Docker
├── postgres/          # Données PostgreSQL (PGDATA)
├── redis/            # Données Redis (AOF)
├── pgadmin/          # Configuration pgAdmin
└── localstack/       # Données LocalStack S3
```

### 🌐 Services et ports

| Service | Port | Accès | Credentials |
|---------|------|-------|-------------|
| PostgreSQL | 5432 | localhost:5432 | filesync / filesync_password |
| Redis | 6379 | localhost:6379 | password: redis_password |
| pgAdmin | 5050 | http://localhost:5050 | admin@filesync.local / admin |
| Redis Commander | 8081 | http://localhost:8081 | - |
| API | 5000 | http://localhost:5000 | - |
| LocalStack S3 | 4566 | localhost:4566 | test / test |

### 📝 Connection Strings

**PostgreSQL :**
```
Host=localhost;Port=5432;Database=filesyncdb;Username=filesync;Password=filesync_password
```

**Redis :**
```
localhost:6379,password=redis_password
```

### 🎯 Modes de développement

#### Mode 1 : Bases de données uniquement
```bash
make dev
# ou
./scripts/start-dev.sh
# ou
docker compose up -d
```
→ PostgreSQL + Redis en Docker, API en local avec `dotnet run`

#### Mode 2 : Avec outils UI
```bash
make dev-tools
# ou
docker compose --profile tools up -d
```
→ Mode 1 + pgAdmin + Redis Commander

#### Mode 3 : Environnement complet
```bash
make dev-full
# ou
./scripts/start-dev-full.sh
# ou
docker compose -f docker-compose.dev.yml up -d
```
→ Tout en Docker : DB + Redis + LocalStack S3 + API

### 🔒 Sécurité

- ✅ Utilisateur non-root dans les conteneurs
- ✅ Fichiers `.env` exclus du contrôle de version
- ✅ Mots de passe configurables via variables d'environnement
- ✅ Network isolé pour les services
- ✅ Health checks pour tous les services critiques

### 📊 Avantages

1. **Développement simplifié** : 3 options de démarrage selon les besoins
2. **Isolation** : Services dans des conteneurs, pas de pollution de l'hôte
3. **Reproductibilité** : Même environnement pour tous les développeurs
4. **Flexibilité** : PostgreSQL/Redis en Docker, API locale ou conteneurisée
5. **Outils UI** : pgAdmin et Redis Commander optionnels
6. **S3 local** : LocalStack pour tester sans AWS
7. **Documentation complète** : README, QUICKSTART, docs/docker-setup.md
8. **Automatisation** : Scripts shell + Makefile

### 🔄 Migration depuis SQLite

Pour migrer de SQLite vers PostgreSQL :

1. Démarrer PostgreSQL :
   ```bash
   make dev
   ```

2. Mettre à jour `appsettings.Development.json` :
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=filesyncdb;Username=filesync;Password=filesync_password"
     },
     "UseInMemoryDatabase": false
   }
   ```

3. Créer et appliquer les migrations :
   ```bash
   make migration  # Créer InitialCreate
   make migrate    # Appliquer
   ```

### 🧹 Maintenance

**Nettoyer les données :**
```bash
make clean
# Supprime les conteneurs, volumes et dossier data/
```

**Rebuilder l'image Docker :**
```bash
make docker-build
```

**Voir les logs :**
```bash
make logs
```

**Vérifier la santé :**
```bash
make health
```

### 📚 Ressources

- [README.md](README.md) - Documentation principale
- [QUICKSTART.md](QUICKSTART.md) - Démarrage rapide
- [docs/docker-setup.md](docs/docker-setup.md) - Documentation Docker détaillée
- [.env.example](.env.example) - Template de configuration

---

**Note :** Cette configuration a été créée pour faciliter le développement local tout en restant proche d'un environnement de production. Les données sont persistées localement dans le dossier `data/` qui est exclu du contrôle de version.
