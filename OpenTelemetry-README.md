# OpenTelemetry with Prometheus and Grafana Setup

This document explains the OpenTelemetry observability setup for your Aspire application.

## Overview

The application now includes comprehensive observability with:
- **OpenTelemetry** for metrics and tracing
- **Prometheus** for metrics collection and storage
- **Grafana** for metrics visualization and dashboards

## Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   .NET Services │    │   Prometheus    │    │     Grafana     │
│                 │    │                 │    │                 │
│ • MasterData    │───▶│ • Scrapes       │───▶│ • Dashboards    │
│ • WeatherAPI    │    │   metrics       │    │ • Visualization │
│ • YarpGateway   │    │ • Stores data   │    │ • Alerts        │
│ • Notification  │    │ • Query engine  │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Services with OpenTelemetry

### 1. MasterDataService
- **Metrics**: HTTP requests, SQL queries, Redis operations
- **Tracing**: ASP.NET Core, HTTP client, SQL client, Redis
- **Endpoint**: `/metrics` (Prometheus format)

### 2. WeatherAPI
- **Metrics**: HTTP requests, runtime metrics
- **Tracing**: ASP.NET Core, HTTP client
- **Endpoint**: `/metrics` (Prometheus format)

### 3. YarpGateway
- **Metrics**: HTTP requests, proxy metrics
- **Tracing**: ASP.NET Core, HTTP client
- **Endpoint**: `/metrics` (Prometheus format)

### 4. NotificationHubService
- **Metrics**: HTTP requests, SignalR metrics
- **Tracing**: ASP.NET Core, HTTP client
- **Endpoint**: `/metrics` (Prometheus format)

## Running the Application

1. **Start the AppHost**:
   ```bash
   cd AspireApp.AppHost
   dotnet run
   ```

2. **Access Services**:
   - **Aspire Dashboard**: http://localhost:15262
   - **Prometheus**: http://localhost:9090
   - **Grafana**: http://localhost:3000 (admin/admin)

## Prometheus Configuration

The Prometheus configuration (`prometheus.yml`) is set up to scrape metrics from:
- All .NET services on their respective ports
- Redis (if available)
- RabbitMQ (if available)

### Key Metrics Collected

- **HTTP Metrics**:
  - `http_requests_total` - Total HTTP requests
  - `http_request_duration_seconds` - Request duration
  - `http_requests_active` - Active requests

- **Process Metrics**:
  - `process_cpu_seconds_total` - CPU usage
  - `process_resident_memory_bytes` - Memory usage

- **Runtime Metrics**:
  - GC metrics
  - Thread pool metrics
  - Exception metrics

## Grafana Dashboards

### Default Dashboard: "Aspire .NET Services Dashboard"

The dashboard includes panels for:
1. **HTTP Request Rate** - Requests per second per service
2. **HTTP Request Duration** - 50th and 95th percentile response times
3. **Active Requests** - Currently active HTTP requests
4. **Error Rate** - 4xx and 5xx error rates
5. **Process CPU Usage** - CPU utilization per service
6. **Process Memory Usage** - Memory consumption per service

### Adding Custom Dashboards

1. Log into Grafana (admin/admin)
2. Go to Dashboards → Import
3. Upload the JSON dashboard file from `grafana/dashboards/`

## Querying Metrics

### Example Prometheus Queries

```promql
# Request rate for all services
rate(http_requests_total[5m])

# 95th percentile response time
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))

# Error rate
rate(http_requests_total{status_code=~"5.."}[5m])

# Memory usage in MB
process_resident_memory_bytes / 1024 / 1024
```

## Custom Metrics

You can add custom metrics in your services:

```csharp
// In your service
private static readonly Counter _customCounter = Metrics.CreateCounter("my_custom_counter", "Description");

// Increment the counter
_customCounter.Add(1);
```

## Troubleshooting

### Prometheus Not Scraping
1. Check if services are running on expected ports
2. Verify `/metrics` endpoint is accessible
3. Check Prometheus logs for scraping errors

### Grafana No Data
1. Verify Prometheus datasource is configured
2. Check if Prometheus has data
3. Verify time range in Grafana

### Services Not Starting
1. Check OpenTelemetry package versions
2. Verify all required packages are installed
3. Check service logs for configuration errors

## Configuration Files

- **Prometheus**: `AspireApp.AppHost/prometheus/prometheus.yml`
- **Grafana Datasources**: `AspireApp.AppHost/grafana/datasources/prometheus.yml`
- **Grafana Dashboards**: `AspireApp.AppHost/grafana/dashboards/aspire-dashboard.json`

## Next Steps

1. **Add Alerts**: Configure Prometheus alerting rules
2. **Custom Dashboards**: Create service-specific dashboards
3. **Log Aggregation**: Add centralized logging (e.g., ELK stack)
4. **Distributed Tracing**: Add Jaeger or Zipkin for trace visualization
5. **Service Mesh**: Consider adding Istio for advanced observability

## Useful Commands

```bash
# Check if metrics endpoints are working
curl http://localhost:5316/metrics  # MasterDataService
curl http://localhost:5062/metrics  # WeatherAPI
curl http://localhost:5211/metrics  # YarpGateway
curl http://localhost:5275/metrics  # NotificationHubService

# Check Prometheus targets
curl http://localhost:9090/api/v1/targets

# Check Prometheus metrics
curl http://localhost:9090/api/v1/query?query=up
``` 