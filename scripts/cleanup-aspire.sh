#!/bin/bash

echo "🧹 Cleaning up Aspire containers..."

# Stop and remove Aspire containers
echo "Stopping Aspire containers..."
docker ps --filter "name=-5108c96d" --format "{{.Names}}" | xargs -r docker stop

echo "Removing Aspire containers..."
docker ps -a --filter "name=-5108c96d" --format "{{.Names}}" | xargs -r docker rm

# Kill any dotnet processes
echo "Killing dotnet processes..."
pkill -f "dotnet run" || true

# Check if ports are free
echo "Checking ports..."
for port in 15262 5211 5316 5062 7122 3000 9090 16686; do
    if lsof -ti:$port > /dev/null 2>&1; then
        echo "⚠️  Port $port is still in use"
    else
        echo "✅ Port $port is free"
    fi
done

echo "�� Cleanup complete!" 