#!/bin/bash

echo "🛑 Stopping All Aspire Infrastructure Services..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running."
    exit 1
fi

# Stop all services
echo "🔄 Stopping all containers..."
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml down

echo "🧹 Cleaning up..."
docker system prune -f

echo "✅ All services stopped and cleaned up!"
echo ""
echo "💡 To start services again:"
echo "   - For Aspire development: ./start-aspire.sh"
echo "   - For full Docker: ./start-full-docker.sh"



