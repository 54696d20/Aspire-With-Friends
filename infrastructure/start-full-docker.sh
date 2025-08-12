#!/bin/bash

echo "🚀 Starting Full Docker Deployment (Infrastructure + Application)..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Load environment variables
if [ -f .env ]; then
    echo "📋 Loading environment configuration..."
    export $(cat .env | grep -v '^#' | xargs)
else
    echo "⚠️  No .env file found, using defaults..."
fi

# Stop any existing containers
echo "🛑 Stopping existing containers..."
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml down

# Start the base infrastructure services
echo "🔧 Starting base infrastructure services..."
docker-compose -f docker-compose.base.yml up -d

# Wait for base services to be healthy
echo "⏳ Waiting for base services to be healthy..."
sleep 15

# Start monitoring services
echo "📊 Starting monitoring services..."
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml up -d

# Wait for monitoring to be healthy
echo "⏳ Waiting for monitoring services to be healthy..."
sleep 10

# Start application services
echo "🚀 Starting application services..."
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml up -d

# Wait for all services to be healthy
echo "⏳ Waiting for all services to be healthy..."
sleep 15

# Check service status
echo "📊 Full Service Status:"
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml ps

echo ""
echo "✅ Full Docker deployment is ready!"
echo ""
echo "🌐 Service URLs:"
echo "   SQL Server: localhost:1433"
echo "   Redis: localhost:6379 (Password: devpassword)"
echo "   Redis Insight: http://localhost:8001"
echo "   RabbitMQ: localhost:5672 (devuser/devpassword)"
echo "   RabbitMQ Management: http://localhost:15672"
echo "   Prometheus: http://localhost:9090"
echo "   Grafana: http://localhost:3000 (admin/admin)"
echo "   PostgreSQL: localhost:5432 (keycloak/secret)"
echo "   Keycloak: https://localhost:8443 (admin/admin) - Accept self-signed certificate"
echo ""
echo "   MasterDataService: http://localhost:5316"
echo "   WeatherAPI: http://localhost:5062"
echo "   Gateway: http://localhost:5211"
echo "   WebWasm: http://localhost:5071"
echo "   NotificationHubService: http://localhost:5275"
echo ""
echo "📝 To view logs: docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml logs -f [service-name]"
echo "🛑 To stop: docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml down"



