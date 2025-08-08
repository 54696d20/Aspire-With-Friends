var builder = DistributedApplication.CreateBuilder(args);

//Add Cache
var cachePassword = builder.AddParameter("CachePassword");
var cache = builder.AddRedis("cache", 6379, cachePassword)
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("REDIS_ARGS", "--requirepass \"\""); // disables auth

//Add SQL
// var dbPassword = builder.AddParameter("DbPassword");
// var sqlServer = builder.AddSqlServer("sqlserver", dbPassword, port:1433)
//     .WithEnvironment("ACCEPT_EULA", "Y")
//     .WithVolume("sql-data", "/var/opt/mssql")
//     .AddDatabase("masterdatadb");

//Add RabbbitMQ
var rabbitUser = builder.AddParameter("RabbitUser");
var rabbitPass = builder.AddParameter("RabbitPass");
var rabbit = builder.AddRabbitMQ("rabbitmq", rabbitUser, rabbitPass, 5672)
    .WithImage("rabbitmq:3-management");

//Add Prometheus
var prometheus = builder.AddContainer("prometheus", "prom/prometheus:latest")
    .WithEnvironment("PROMETHEUS_CONFIG_FILE", "/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(9090, name: "prometheus", targetPort: 9090)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount(Path.GetFullPath("prometheus"), "/etc/prometheus");

//Add Grafana
var grafana = builder.AddContainer("grafana", "grafana/grafana:latest")
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
    .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false")
    .WithHttpEndpoint(3000, name: "grafana", targetPort: 3000)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount(Path.GetFullPath("grafana/datasources"), "/etc/grafana/provisioning/datasources")
    .WithBindMount(Path.GetFullPath("grafana/dashboards"), "/etc/grafana/provisioning/dashboards");

//OpenTelemetry Collector temporarily removed - will add back once basic services are working

// Simple working configuration - no complex dependencies
builder.AddProject<Projects.AspireApp_MasterDataService>("masterdataservice");

builder.AddProject<Projects.AspireApp_WeatherAPI>("weatherapi");

builder.AddProject<Projects.YarpGateway>("gateway");

builder.AddProject<Projects.AspireApp_WebWasm>("aspireapp-webwasm")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireApp_NotificationHubService>("notificationhubservice");

builder.Build().Run();
