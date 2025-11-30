# Database Scripts - FileSyncHub

Ce dossier contient les scripts SQL et la documentation pour la gestion de la base de données FileSyncHub.

## 📁 Fichiers

### init-db.sql
Script d'initialisation de la base de données PostgreSQL.
- Création des extensions PostgreSQL nécessaires
- Configuration des privilèges utilisateur
- Commentaires sur la base de données

### migration-script.sql
Script SQL complet généré à partir de la migration EF Core `InitialCreate`.
- Création de toutes les tables
- Création de tous les index
- Contraintes de clés étrangères
- Commentaires sur les tables

Ce script peut être utilisé pour créer manuellement le schéma de base de données sans utiliser EF Core migrations.

## 🚀 Utilisation

### Option 1 : Utiliser EF Core Migrations (Recommandé)

```bash
# 1. Démarrer PostgreSQL avec Docker
docker compose up -d postgres

# 2. Appliquer les migrations
dotnet ef database update \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web

# Ou utiliser le Makefile
make migrate
```

### Option 2 : Utiliser les scripts SQL manuellement

```bash
# 1. Démarrer PostgreSQL avec Docker
docker compose up -d postgres

# 2. Se connecter à PostgreSQL
docker exec -it filesync-postgres psql -U filesync -d filesyncdb

# 3. Exécuter le script de migration
\i /path/to/migration-script.sql
```

### Option 3 : Exécuter depuis l'hôte

```bash
# 1. Démarrer PostgreSQL
docker compose up -d postgres

# 2. Exécuter le script
psql -h localhost -p 5432 -U filesync -d filesyncdb -f database/migration-script.sql
# Mot de passe : filesync_password
```

## 📊 Schéma de base de données

### Tables principales

#### Documents
Stocke les métadonnées des documents.

**Colonnes principales :**
- `Id` (uuid, PK) - Identifiant unique
- `OrganizationId` (varchar 100) - ID de l'organisation
- `Title` (varchar 500) - Titre du document
- `FileName` (varchar 500) - Nom du fichier
- `FileSize` (bigint) - Taille en octets
- `Category` (integer) - Catégorie (enum)
- `Status` (integer) - Statut (enum)
- `CurrentVersion` (integer) - Version actuelle
- `S3Key` (varchar 1000) - Clé S3
- `WorkflowInstanceId` (varchar 100) - ID du workflow
- Champs d'audit : `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`

**Index :**
- `OrganizationId`
- `Category`
- `Status`
- `WorkflowInstanceId`
- `(OrganizationId, Category)` - Composite

#### DocumentVersions
Stocke les versions des documents.

**Colonnes principales :**
- `Id` (uuid, PK) - Identifiant unique
- `DocumentId` (uuid, FK) - Référence au document
- `VersionNumber` (integer) - Numéro de version
- `S3Key` (varchar 1000) - Clé S3 de la version
- `FileName` (varchar 500) - Nom du fichier
- `IsActive` (boolean) - Version active
- Champs d'audit

**Index :**
- `DocumentId`
- `(DocumentId, VersionNumber)` - Unique composite

#### DocumentStatusHistory
Historique des changements de statut.

**Colonnes principales :**
- `Id` (uuid, PK)
- `DocumentId` (uuid, FK)
- `Status` (integer) - Nouveau statut
- `Comment` (varchar 1000) - Commentaire
- `ChangedBy` (varchar 200) - Utilisateur
- Champs d'audit

**Index :**
- `DocumentId`
- `(DocumentId, CreatedAt)`

#### OrganizationVersioningConfigurations
Configuration du versioning par organisation.

**Colonnes principales :**
- `Id` (uuid, PK)
- `OrganizationId` (varchar 100) - Unique
- `DefaultVersioningEnabled` (boolean)
- `DefaultMaxVersions` (integer)
- `IsActive` (boolean)
- Champs d'audit

**Index :**
- `OrganizationId` - Unique
- `IsActive`

#### CategoryVersioningConfigurations
Configuration du versioning par catégorie.

**Colonnes principales :**
- `Id` (uuid, PK)
- `OrganizationVersioningConfigurationId` (uuid, FK)
- `Category` (integer)
- `VersioningEnabled` (boolean)
- `MaxVersions` (integer)
- Champs d'audit

**Index :**
- `(OrganizationVersioningConfigurationId, Category)` - Unique composite

## 🔄 Gestion des migrations

### Créer une nouvelle migration

```bash
# Créer une migration
dotnet ef migrations add MigrationName \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web \
  --output-dir Persistence/Migrations

# Ou avec le Makefile
make migration
# Entrer le nom de la migration quand demandé
```

### Appliquer les migrations

```bash
# Appliquer toutes les migrations
dotnet ef database update \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web

# Ou avec le Makefile
make migrate
```

### Revenir à une migration précédente

```bash
# Revenir à une migration spécifique
dotnet ef database update MigrationName \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web

# Revenir à l'état vide
dotnet ef database update 0 \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web
```

### Générer un script SQL

```bash
# Générer un script pour toutes les migrations
dotnet ef migrations script \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web \
  --output database/migration-script.sql

# Générer un script pour une migration spécifique
dotnet ef migrations script InitialCreate NextMigration \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web
```

### Supprimer la dernière migration

```bash
# Supprimer la dernière migration (non appliquée)
dotnet ef migrations remove \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web
```

### Lister les migrations

```bash
# Voir toutes les migrations
dotnet ef migrations list \
  --project src/Dotland.FileSyncHub.Infrastructure \
  --startup-project src/Dotland.FileSyncHub.Web
```

## 📋 Checklist avant production

- [ ] Vérifier que toutes les migrations sont appliquées
- [ ] Créer des backups de la base de données
- [ ] Tester la migration sur un environnement de staging
- [ ] Vérifier les index pour les performances
- [ ] Configurer les politiques de sauvegarde
- [ ] Configurer la réplication (si nécessaire)
- [ ] Configurer le monitoring des requêtes lentes
- [ ] Documenter le schéma et les procédures

## 🔍 Commandes utiles PostgreSQL

```sql
-- Voir toutes les tables
\dt

-- Voir la structure d'une table
\d "Documents"

-- Voir tous les index
\di

-- Voir la taille des tables
SELECT
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;

-- Voir les connexions actives
SELECT * FROM pg_stat_activity WHERE datname = 'filesyncdb';

-- Voir les migrations appliquées (table EF Core)
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
```

## 🛠️ Maintenance

### Backup

```bash
# Backup complet
docker exec filesync-postgres pg_dump -U filesync filesyncdb > backup.sql

# Backup avec compression
docker exec filesync-postgres pg_dump -U filesync filesyncdb | gzip > backup.sql.gz

# Backup de données uniquement (sans schéma)
docker exec filesync-postgres pg_dump -U filesync -a filesyncdb > data-only.sql
```

### Restore

```bash
# Restaurer depuis un backup
docker exec -i filesync-postgres psql -U filesync -d filesyncdb < backup.sql

# Restaurer depuis un fichier compressé
gunzip < backup.sql.gz | docker exec -i filesync-postgres psql -U filesync -d filesyncdb
```

### Vacuum et Analyze

```sql
-- Vacuum complet
VACUUM FULL;

-- Analyze pour mettre à jour les statistiques
ANALYZE;

-- Vacuum et Analyze ensemble
VACUUM ANALYZE;
```

## 📚 Ressources

- [PostgreSQL Documentation](https://www.postgresql.org/docs/16/)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Npgsql Documentation](https://www.npgsql.org/efcore/)
