#!/bin/bash

set -e

echo "🚀 Starting FileSyncHub Full Development Environment"
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

echo "🐳 Starting all services (PostgreSQL, Redis, LocalStack, API)..."
docker compose -f docker-compose.dev.yml up -d

echo ""
echo "⏳ Waiting for services to be healthy..."
sleep 10

# Check services
echo "Checking services status..."
docker compose -f docker-compose.dev.yml ps

echo ""
echo "✅ All services started!"
echo ""
echo "📊 Services Available:"
echo "  PostgreSQL: localhost:5432"
echo "  Redis: localhost:6379"
echo "  LocalStack (S3): localhost:4566"
echo "  API: http://localhost:5000"
echo "  Health Check: http://localhost:5000/health"
echo ""
echo "🪣 Initialize S3 bucket in LocalStack:"
echo "  docker exec filesync-localstack awslocal s3 mb s3://filesync-dev"
echo "  docker exec filesync-localstack awslocal s3 ls"
echo ""
echo "📌 To stop services: docker compose -f docker-compose.dev.yml down"
echo "📌 To view logs: docker compose -f docker-compose.dev.yml logs -f"
