-- FileSyncHub Database Initialization Script
-- PostgreSQL 16+

-- Drop database if exists (for development only)
-- DROP DATABASE IF EXISTS filesyncdb;

-- Create database
-- CREATE DATABASE filesyncdb
--     WITH
--     OWNER = filesync
--     ENCODING = 'UTF8'
--     LC_COLLATE = 'en_US.utf8'
--     LC_CTYPE = 'en_US.utf8'
--     TABLESPACE = pg_default
--     CONNECTION LIMIT = -1;

-- Connect to the database
\c filesyncdb;

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- For text search

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE filesyncdb TO filesync;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO filesync;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO filesync;

-- Note: The actual tables will be created by EF Core migrations
-- Run: dotnet ef database update --project src/Dotland.FileSyncHub.Infrastructure --startup-project src/Dotland.FileSyncHub.Web

COMMENT ON DATABASE filesyncdb IS 'FileSyncHub - Document Management System Database';
