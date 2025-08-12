var builder = DistributedApplication.CreateBuilder(args);

// Enterprise Configuration - Infrastructure services are external
// These will be managed by the infrastructure docker-compose
// No need to add Redis/RabbitMQ here as they're external dependencies

// Business Application Services - This is your core product
builder.AddProject<Projects.AspireApp_MasterDataService>("masterdataservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireApp_WeatherAPI>("weatherapi")
    .WithHttpHealthCheck("/api/weather/health");

builder.AddProject<Projects.YarpGateway>("gateway")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireApp_WebWasm>("aspireapp-webwasm")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireApp_NotificationHubService>("notificationhubservice")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
