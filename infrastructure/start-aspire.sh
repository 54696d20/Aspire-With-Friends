#!/bin/bash

echo "🚀 Starting Aspire Infrastructure Services (Infrastructure Only)..."

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
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml down

# Start the base infrastructure services
echo "🔧 Starting base infrastructure services..."
docker-compose -f docker-compose.base.yml up -d

# Wait for base services to be healthy
echo "⏳ Waiting for base services to be healthy..."
sleep 15

# Start monitoring services
echo "📊 Starting monitoring services..."
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml up -d

# Wait for all services to be healthy
echo "⏳ Waiting for all services to be healthy..."
sleep 10

# Check service status
echo "📊 Infrastructure Service Status:"
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml ps

echo ""
echo "✅ Infrastructure services are ready for Aspire!"
echo ""
echo "🌐 Service URLs:"
echo "   SQL Server: localhost:1433"
echo "   Redis: localhost:6379 (Password: ${REDIS_PASSWORD:-devpassword})"
echo "   Redis Insight: http://localhost:8001"
echo "   RabbitMQ: localhost:5672 (${RABBITMQ_USER:-devuser}/${RABBITMQ_PASSWORD:-devpassword})"
echo "   RabbitMQ Management: http://localhost:15672"
echo "   Prometheus: http://localhost:9090"
echo "   Grafana: http://localhost:3000 (admin/${GRAFANA_ADMIN_PASSWORD:-admin})"
echo ""
echo "📝 To view logs: docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml logs -f [service-name]"
echo "🛑 To stop: docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml down"
echo ""
echo "🎯 Now you can start your Aspire AppHost - it will connect to these external services!"



