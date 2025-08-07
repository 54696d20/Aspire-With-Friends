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

## Initial Setup (First Time Only)

### Step 1: Configure Prometheus Datasource in Grafana
1. Go to http://localhost:3000
2. Login with admin/admin
3. Click the **gear icon** (Configuration) → **Data Sources**
4. Click **"Add data source"**
5. Select **"Prometheus"**
6. Set URL to: `http://prometheus:9090`
7. Click **"Save & test"**
8. You should see: **"Data source is working"**

### Step 2: Test Data Availability
1. Click the **compass icon** (Explore)
2. Make sure **Prometheus** is selected
3. Type query: `up`
4. Click **Run query**
5. You should see data for all your services

### Step 3: Import Dashboards (Optional)
1. Go to **Dashboards** → **Import**
2. Click **"Upload JSON file"**
3. Upload these files one by one:
   - `AspireApp.AppHost/grafana/dashboards/simple-test.json`
   - `AspireApp.AppHost/grafana/dashboards/basic-metrics.json` 
   - `AspireApp.AppHost/grafana/dashboards/aspire-dashboard.json`
4. Select **Prometheus** as the datasource for each
5. Click **Import**

## Prometheus Configuration

The Prometheus configuration (`prometheus.yml`) is set up to scrape metrics from:
- All .NET services on their respective ports
- Redis (if available)
- RabbitMQ (if available)

### Key Metrics Collected

- **HTTP Metrics**:
  - `http_server_request_duration_seconds_count` - Total HTTP requests
  - `http_server_request_duration_seconds` - Request duration
  - `http_server_request_duration_seconds_sum` - Sum of request durations

- **Process Metrics**:
  - `process_cpu_seconds_total` - CPU usage
  - `process_working_set_bytes` - Memory usage

- **Runtime Metrics**:
  - GC metrics
  - Thread pool metrics
  - Exception metrics

## Grafana Dashboards

### Available Dashboards

1. **Simple Test Dashboard** - Basic metrics verification
2. **Basic Metrics Dashboard** - Core service metrics
3. **Aspire .NET Services Dashboard** - Comprehensive service monitoring

### Dashboard Panels Include:
1. **HTTP Request Rate** - Requests per second per service
2. **HTTP Request Duration** - 50th and 95th percentile response times
3. **Active Requests** - Currently active HTTP requests
4. **Error Rate** - 4xx and 5xx error rates
5. **Process CPU Usage** - CPU utilization per service
6. **Process Memory Usage** - Memory consumption per service

## Querying Metrics

### Example Prometheus Queries

```promql
# Service status (up/down)
up

# Request rate for all services
rate(http_server_request_duration_seconds_count[5m])

# 95th percentile response time
histogram_quantile(0.95, rate(http_server_request_duration_seconds_bucket[5m]))

# Error rate (5xx errors)
rate(http_server_request_duration_seconds_count{http_status_code=~"5.."}[5m])

# Memory usage in MB
process_working_set_bytes / 1024 / 1024

# CPU usage
rate(process_cpu_seconds_total[5m])
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
2. Verify `/metrics` endpoint is accessible: `curl http://localhost:5316/metrics`
3. Check Prometheus logs for scraping errors
4. Verify `host.docker.internal` is used in prometheus.yml for .NET services

### Grafana No Data
1. Verify Prometheus datasource is configured with URL: `http://prometheus:9090`
2. Check if Prometheus has data by querying `up` in Explore
3. Verify time range in Grafana (try last 1 hour)
4. Check if services are generating traffic

### Services Not Starting
1. Check OpenTelemetry package versions (should be 1.12.0 for core packages)
2. Verify all required packages are installed
3. Check service logs for configuration errors
4. Ensure `app.MapPrometheusScrapingEndpoint()` is called in Program.cs

### Grafana Container Issues
1. If Grafana exits with error, check container logs
2. Verify no conflicting volume mounts
3. Ensure proper environment variables are set

## Configuration Files

- **Prometheus**: `AspireApp.AppHost/prometheus/prometheus.yml`
- **Grafana Dashboards**: `AspireApp.AppHost/grafana/dashboards/`

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

# Generate some traffic to see metrics
curl http://localhost:5316/api/locations  # MasterDataService
curl http://localhost:5062/weatherforecast  # WeatherAPI
```

## Current Status

✅ **Working Components:**
- OpenTelemetry instrumentation in all .NET services
- Prometheus container with proper scraping configuration
- Grafana container with manual datasource setup
- Metrics endpoints exposed on all services
- Docker networking configured correctly

🔄 **Manual Steps Required:**
- Initial Grafana datasource configuration (one-time setup)
- Dashboard import (optional)

🚀 **Ready for Production:**
- All core observability components are functional
- Metrics are being collected and stored
- Visualization is available through Grafana 