# Aspire Infrastructure Services

This directory contains the infrastructure services for the Aspire application, separated from the application code for enterprise-level architecture.

## 🏗️ Architecture

- **Infrastructure services** are managed separately from the application
- **Modular docker-compose** approach for different deployment scenarios
- **Environment-driven configuration** using .env files
- **Health checks** configured for all services
- **Monitoring** (Prometheus + Grafana) is external and can be scaled independently

## 🚀 Deployment Scenarios

### 1. **Aspire Development** (Infrastructure Only)
Use when running the Aspire AppHost locally:
```bash
cd infrastructure
./start-aspire.sh
```
- Starts: SQL Server, Redis, RabbitMQ, Prometheus, Grafana
- **You start the Aspire AppHost separately** - it connects to these external services

### 2. **Full Docker Deployment** (Infrastructure + Applications)
Use when you want everything in Docker:
```bash
cd infrastructure
./start-full-docker.sh
```
- Starts: All infrastructure + all application services
- **Everything runs in containers** - no need for Aspire

### 3. **Custom Scenarios**
Mix and match compose files as needed:
```bash
# Just infrastructure (no monitoring)
docker-compose -f docker-compose.base.yml up -d

# Infrastructure + monitoring
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml up -d

# Everything
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml up -d
```

## 📊 Services

| Service | Port | URL | Credentials |
|---------|------|-----|-------------|
| SQL Server | 1433 | - | `sa`/`${SQLSERVER_SA_PASSWORD}` |
| Redis | 6379 | - | Password: `${REDIS_PASSWORD}` |
| Redis Insight | 8001 | http://localhost:8001 | - |
| RabbitMQ | 5672 | - | `${RABBITMQ_USER}`/`${RABBITMQ_PASSWORD}` |
| RabbitMQ Management | 15672 | http://localhost:15672 | `${RABBITMQ_USER}`/`${RABBITMQ_PASSWORD}` |
| Prometheus | 9090 | http://localhost:9090 | - |
| Grafana | 3000 | http://localhost:3000 | `admin`/`${GRAFANA_ADMIN_PASSWORD}` |

## 🔧 Configuration

### Environment Variables
Copy `env.example` to `.env` and customize:
```bash
cp env.example .env
# Edit .env with your values
```

### Default Values
- `SQLSERVER_SA_PASSWORD=P@ssword123!`
- `REDIS_PASSWORD=devpassword`
- `RABBITMQ_USER=devuser`
- `RABBITMQ_PASSWORD=devpassword`
- `GRAFANA_ADMIN_PASSWORD=admin`
- `ENVIRONMENT=development`

### Prometheus
- Scrapes metrics from all .NET services using HTTPS
- Configured to use `host.docker.internal` to reach host services
- TLS verification disabled for development (use proper certificates in production)

### Grafana
- Pre-configured dashboards for service monitoring
- Prometheus data source automatically configured
- Dashboard provisioning enabled

## 📈 Monitoring

The infrastructure automatically monitors:
- **Service health** (up/down status)
- **Performance metrics** (CPU, memory, response times)
- **Infrastructure health** (Redis, RabbitMQ status)

## 🏢 Enterprise Benefits

1. **Separation of Concerns**: Infrastructure separate from application code
2. **Flexible Deployment**: Choose your deployment scenario
3. **Environment Configuration**: Easy to customize per environment
4. **Independent Scaling**: Monitoring can scale independently of application
5. **Easier Maintenance**: Infrastructure changes don't require application rebuilds
6. **Production Ready**: Same setup can be used in production environments
7. **Security**: Infrastructure services can be secured independently

## 🔒 Security Notes

- **Development credentials** are used for local development
- **HTTPS** is configured for all service communication
- **TLS verification** is disabled for development (enable in production)
- **Network isolation** using Docker networks
- **Environment variables** for sensitive configuration

## 🚨 Production Considerations

1. **Use proper SSL certificates** instead of `insecure_skip_verify: true`
2. **Secure credentials** for all services
3. **Network policies** to restrict access
4. **Monitoring alerts** for service failures
5. **Backup strategies** for persistent data
6. **Use production .env files** with secure values

## 🧹 Cleanup

### Stop All Services
```bash
# Stop everything
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml down

# Or use the stop script
./stop-all.sh
```

### Remove Volumes (Data Loss!)
```bash
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml -f docker-compose.applications.yml down -v
```

## 🔄 Migration from Old Setup

If you were using the old docker-compose files:

1. **Stop old services**: `docker-compose down` (in project root)
2. **Start new infrastructure**: `cd infrastructure && ./start-aspire.sh`
3. **Start Aspire AppHost**: `dotnet run --project AspireApp.AppHost`

The AppHost will now connect to the external infrastructure services instead of managing them internally.
