.PHONY: help dev dev-tools up down build logs clean migrate test restore run

# Default target
help:
	@echo "FileSyncHub - Makefile commands"
	@echo ""
	@echo "Available commands:"
	@echo "  make dev          - Start PostgreSQL and Redis only"
	@echo "  make dev-tools    - Start databases with management tools (pgAdmin, Redis Commander)"
	@echo "  make dev-full     - Start all services including API and LocalStack (uses docker compose.dev.yml)"
	@echo "  make up           - Start all services (alias for dev)"
	@echo "  make down         - Stop all services"
	@echo "  make build        - Build .NET solution"
	@echo "  make logs         - Show logs from all services"
	@echo "  make clean        - Stop services and remove volumes (⚠️  deletes data)"
	@echo "  make migrate      - Run EF Core migrations"
	@echo "  make test         - Run tests"
	@echo "  make restore      - Restore NuGet packages"
	@echo "  make run          - Run the API locally (without Docker)"
	@echo "  make docker-build - Build Docker image for the API"
	@echo ""

# Start only databases (PostgreSQL + Redis)
dev:
	docker compose up -d postgres redis

# Start databases with management tools
dev-tools:
	docker compose --profile tools up -d

# Start full development environment (with API and LocalStack)
dev-full:
	docker compose -f docker compose.dev.yml up -d

# Alias for dev
up: dev

# Stop all services
down:
	docker compose down
	docker compose -f docker compose.dev.yml down 2>/dev/null || true

# Build .NET solution
build:
	dotnet build

# Show logs
logs:
	docker compose logs -f

# Clean (stop and remove volumes)
clean:
	docker compose down -v
	docker compose -f docker compose.dev.yml down -v 2>/dev/null || true
	rm -rf data/

# Run migrations
migrate:
	dotnet ef database update --project src/Dotland.FileSyncHub.Infrastructure --startup-project src/Dotland.FileSyncHub.Web

# Create new migration
migration:
	@read -p "Enter migration name: " name; \
	dotnet ef migrations add $$name --project src/Dotland.FileSyncHub.Infrastructure --startup-project src/Dotland.FileSyncHub.Web

# Run tests
test:
	dotnet test

# Restore NuGet packages
restore:
	dotnet restore

# Run API locally (without Docker)
run:
	dotnet run --project src/Dotland.FileSyncHub.Web

# Run API with hot reload
watch:
	dotnet watch --project src/Dotland.FileSyncHub.Web

# Build Docker image
docker-build:
	docker build -t filesync-api:latest .

# Initialize LocalStack S3 bucket (for dev-full)
init-localstack:
	@echo "Waiting for LocalStack to be ready..."
	@sleep 5
	docker exec filesync-localstack awslocal s3 mb s3://filesync-dev 2>/dev/null || echo "Bucket already exists"
	docker exec filesync-localstack awslocal s3 ls

# Check services health
health:
	@echo "Checking services health..."
	@docker compose ps
	@echo ""
	@echo "PostgreSQL: $(shell docker exec filesync-postgres pg_isready -U filesync 2>&1 || echo "Not running")"
	@echo "Redis: $(shell docker exec filesync-redis redis-cli ping 2>&1 || echo "Not running")"

# Format code
format:
	dotnet format

# Show connection strings
info:
	@echo "PostgreSQL Connection: Host=localhost;Port=5432;Database=filesyncdb;Username=filesync;Password=filesync_password"
	@echo "Redis Connection: localhost:6379,password=redis_password"
	@echo "pgAdmin: http://localhost:5050 (admin@filesync.local / admin)"
	@echo "Redis Commander: http://localhost:8081"
	@echo "API: http://localhost:5000"
	@echo "Health Check: http://localhost:5000/health"
