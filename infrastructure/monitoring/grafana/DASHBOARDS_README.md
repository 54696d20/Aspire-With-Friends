# Enterprise Grafana Dashboards

This directory contains production-ready Grafana dashboards designed for enterprise operations teams. These dashboards provide comprehensive monitoring and alerting capabilities for the Aspire application stack.

## 🎯 Dashboard Overview

### 1. **Enterprise Aspire Dashboard** (`enterprise-aspire-dashboard.json`)
**Purpose**: Application-level monitoring for development and operations teams
**Audience**: DevOps engineers, SREs, application developers
**Focus**: .NET services, performance metrics, error rates, business metrics

### 2. **Infrastructure Health Dashboard** (`infrastructure-health-dashboard.json`)
**Purpose**: Infrastructure service monitoring and health checks
**Audience**: Infrastructure engineers, DevOps engineers
**Focus**: Redis, RabbitMQ, SQL Server, system resources

### 3. **Basic Metrics Debug** (`aspire-metrics-dashboard.json`)
**Purpose**: Simple debugging and basic monitoring
**Audience**: Developers, basic troubleshooting
**Focus**: Service status, basic HTTP metrics

## 🚀 Getting Started

### Automatic Dashboard Provisioning
The dashboards are automatically provisioned when Grafana starts. No manual import required.

### Manual Import (if needed)
1. Go to Grafana → Dashboards → Import
2. Upload the JSON file
3. Select Prometheus as the data source
4. Import

## 📊 Enterprise Aspire Dashboard

### Key Metrics Monitored

#### **Service Health & Status**
- **Service Health Status**: Real-time UP/DOWN status for all services
- **Service Status Timeline**: Historical service availability

#### **Performance Metrics**
- **Request Rate**: HTTP requests per second (5-minute average)
- **Response Time**: 95th percentile response times in milliseconds
- **Error Rate**: Percentage of 5xx errors

#### **Resource Utilization**
- **CPU Usage**: Process CPU consumption
- **Memory Usage**: Resident memory usage
- **Thread Count**: Active thread count
- **Open File Descriptors**: File descriptor usage
- **.NET Total Memory**: .NET runtime memory allocation
- **GC Collections Rate**: Garbage collection frequency

### **Template Variables**
- **Service**: Filter by specific service (MasterDataService, WeatherAPI, Gateway, etc.)
- **Route**: Filter by specific HTTP route/endpoint

### **Thresholds & Alerts**
- **Response Time**: 
  - 🟢 Green: < 100ms
  - 🟡 Yellow: 100-500ms  
  - 🔴 Red: > 500ms
- **Error Rate**:
  - 🟢 Green: < 1%
  - 🔴 Red: > 1%
- **Memory Usage**:
  - 🟢 Green: < 1GB
  - 🟡 Yellow: 1-2GB
  - 🔴 Red: > 2GB

## 🏗️ Infrastructure Health Dashboard

### Key Metrics Monitored

#### **Service Health**
- **Infrastructure Service Health**: Redis, RabbitMQ, Prometheus status
- **Service Status Timeline**: Historical infrastructure availability

#### **Redis Monitoring**
- **Connected Clients**: Number of active Redis connections
- **Memory Usage**: Redis memory consumption in bytes

#### **RabbitMQ Monitoring**
- **Queue Messages**: Messages waiting in queues
- **Active Connections**: Number of active RabbitMQ connections
- **Message Rate**: Messages published, delivered, and acknowledged per second

### **Template Variables**
- **Infrastructure Service**: Filter by Redis, RabbitMQ, or Prometheus

### **Thresholds & Alerts**
- **Redis Memory**:
  - 🟢 Green: < 1GB
  - 🟡 Yellow: 1-5GB
  - 🔴 Red: > 5GB
- **RabbitMQ Connections**:
  - 🟢 Green: < 100
  - 🟡 Yellow: 100-200
  - 🔴 Red: > 200

## 🔍 How to Use for Operations

### **Daily Health Checks**
1. **Open Enterprise Aspire Dashboard**
2. **Check Service Health Status** - All services should show green (UP)
3. **Review Error Rates** - Should be < 1%
4. **Check Response Times** - Should be < 100ms for most endpoints

### **Performance Investigation**
1. **Identify Slow Endpoints** - Look for high response times in "Response Time - 95th Percentile"
2. **Check Resource Usage** - Monitor CPU and memory trends
3. **Analyze Error Patterns** - Look for spikes in error rates

### **Infrastructure Monitoring**
1. **Open Infrastructure Health Dashboard**
2. **Check Redis Health** - Monitor memory usage and connections
3. **Monitor RabbitMQ** - Watch queue depths and message rates
4. **Verify Service Status** - Ensure all infrastructure services are UP

### **Troubleshooting Workflow**
1. **Service Down**: Check service health status and logs
2. **High Response Times**: Investigate CPU, memory, and thread usage
3. **High Error Rates**: Check application logs and database connectivity
4. **Memory Issues**: Monitor GC collections and memory trends

## 📈 Creating Alerts

### **Critical Alerts**
- Service DOWN status
- Error rate > 5%
- Response time > 1 second
- Memory usage > 90%

### **Warning Alerts**
- Error rate > 1%
- Response time > 500ms
- Memory usage > 80%
- High GC collection rate

### **Alert Configuration**
```yaml
# Example Prometheus alert rule
groups:
  - name: aspire-alerts
    rules:
      - alert: HighErrorRate
        expr: rate(http_server_request_duration_seconds_count{http_status_code=~"5.."}[5m]) / rate(http_server_request_duration_seconds_count{http_route!="/metrics"}[5m]) * 100 > 1
        for: 2m
        labels:
          severity: warning
        annotations:
          summary: "High error rate detected"
          description: "Error rate is {{ $value }}% for {{ $labels.job }}"
```

## 🎨 Dashboard Customization

### **Adding New Panels**
1. **Identify the Metric**: Use Prometheus queries to find available metrics
2. **Create Panel**: Add new panel with appropriate visualization
3. **Set Thresholds**: Configure color thresholds for alerts
4. **Add to Dashboard**: Position panel logically in the layout

### **Modifying Existing Panels**
1. **Edit Panel**: Click panel title → Edit
2. **Update Query**: Modify Prometheus expression
3. **Adjust Thresholds**: Update color thresholds
4. **Save Changes**: Apply and save dashboard

### **Best Practices**
- **Consistent Naming**: Use clear, descriptive panel titles
- **Logical Grouping**: Group related metrics together
- **Appropriate Units**: Use correct units (ms, bytes, req/s)
- **Color Coding**: Use consistent color schemes for thresholds

## 🔧 Troubleshooting

### **Dashboard Not Loading**
1. **Check Data Source**: Verify Prometheus connection
2. **Check Metrics**: Ensure services are exposing metrics
3. **Check Permissions**: Verify Grafana user permissions

### **No Data in Panels**
1. **Verify Query**: Check Prometheus expression syntax
2. **Check Time Range**: Ensure time range includes data
3. **Check Labels**: Verify label selectors match actual data

### **Performance Issues**
1. **Reduce Refresh Rate**: Increase dashboard refresh interval
2. **Limit Time Range**: Use shorter time ranges for heavy queries
3. **Optimize Queries**: Use more efficient Prometheus expressions

## 📚 Additional Resources

- **Prometheus Query Language**: [PromQL Documentation](https://prometheus.io/docs/prometheus/latest/querying/)
- **Grafana Documentation**: [Grafana Docs](https://grafana.com/docs/)
- **OpenTelemetry Metrics**: [.NET Metrics](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics)
- **Aspire Monitoring**: [.NET Aspire Monitoring](https://docs.microsoft.com/en-us/dotnet/aspire/monitoring)

## 🚨 Support

For dashboard issues or enhancement requests:
1. **Check Logs**: Review Grafana and Prometheus logs
2. **Verify Configuration**: Ensure dashboard JSON is valid
3. **Test Queries**: Verify Prometheus queries work independently
4. **Contact Team**: Reach out to DevOps/Infrastructure team

---

**Last Updated**: August 2025
**Version**: 1.0
**Maintainer**: DevOps Team

