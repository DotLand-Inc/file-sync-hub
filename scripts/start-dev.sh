#!/bin/bash

set -e

echo "🚀 Starting FileSyncHub Development Environment"
echo ""

# Check if .env exists, if not copy from .env.example
if [ ! -f .env ]; then
    echo "📝 Creating .env file from .env.example..."
    cp .env.example .env
    echo "✓ .env file created. Please review and update if needed."
    echo ""
fi

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

echo "🐳 Starting PostgreSQL and Redis..."
docker compose up -d postgres redis

echo ""
echo "⏳ Waiting for services to be healthy..."
sleep 5

# Check PostgreSQL
if docker exec filesync-postgres pg_isready -U filesync > /dev/null 2>&1; then
    echo "✓ PostgreSQL is ready"
else
    echo "⚠️  PostgreSQL is not ready yet, please wait a moment..."
fi

# Check Redis
if docker exec filesync-redis redis-cli -a redis_password ping > /dev/null 2>&1; then
    echo "✓ Redis is ready"
else
    echo "⚠️  Redis is not ready yet, please wait a moment..."
fi

echo ""
echo "✅ Services started successfully!"
echo ""
echo "📊 Connection Information:"
echo "  PostgreSQL: localhost:5432"
echo "  Redis: localhost:6379"
echo ""
echo "💡 Connection Strings:"
echo "  PostgreSQL: Host=localhost;Port=5432;Database=filesyncdb;Username=filesync;Password=filesync_password"
echo "  Redis: localhost:6379,password=redis_password"
echo ""
echo "🛠️  Next steps:"
echo "  1. Update appsettings.Development.json with the connection strings"
echo "  2. Run migrations: dotnet ef database update --project src/Dotland.FileSyncHub.Infrastructure --startup-project src/Dotland.FileSyncHub.Web"
echo "  3. Start the API: dotnet run --project src/Dotland.FileSyncHub.Web"
echo ""
echo "📌 To stop services: docker compose down"
echo "📌 To view logs: docker compose logs -f"
