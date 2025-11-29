#!/bin/bash

echo "🛑 Stopping FileSyncHub Development Environment..."

docker compose down
docker compose -f docker-compose.dev.yml down 2>/dev/null || true

echo "✅ All services stopped"
