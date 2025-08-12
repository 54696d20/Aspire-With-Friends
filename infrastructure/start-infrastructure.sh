#!/bin/bash

echo "🚀 Starting Aspire Infrastructure Services..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Stop any existing containers
echo "🛑 Stopping existing containers..."
docker-compose down

# Start the infrastructure services
echo "🔧 Starting infrastructure services..."
docker-compose up -d

# Wait for services to be healthy
echo "⏳ Waiting for services to be healthy..."
sleep 10

# Check service status
echo "📊 Infrastructure Service Status:"
docker-compose ps

echo ""
echo "✅ Infrastructure services are starting up!"
echo ""
echo "🌐 Service URLs:"
echo "   Redis: localhost:6379"
echo "   Redis Insight: http://localhost:8001"
echo "   RabbitMQ: localhost:5672"
echo "   RabbitMQ Management: http://localhost:15672 (devuser/devpassword)"
echo "   Prometheus: http://localhost:9090"
echo "   Grafana: http://localhost:3000 (admin/admin)"
echo ""
echo "📝 To view logs: docker-compose logs -f [service-name]"
echo "🛑 To stop: docker-compose down"

