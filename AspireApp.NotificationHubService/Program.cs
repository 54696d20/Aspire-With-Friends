using AspireApp.NotificationHubService.Hubs;
using Wolverine;
using Wolverine.RabbitMQ;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "NotificationHubService", serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()) // For development - see traces in console
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

// Add SignalR
builder.Services.AddSignalR();

// Add Wolverine with RabbitMQ
builder.Host.UseWolverine(opts =>
{
    var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq");
    if (!string.IsNullOrEmpty(rabbitMqConnectionString))
    {
        opts.UseRabbitMq(rabbitMqConnectionString: rabbitMqConnectionString)
            .AutoPurgeOnStartup()
            .AutoProvision();
        
        opts.ListenToRabbitQueue("wolverine");
    }
    else
    {
        // Fallback to local queue if RabbitMQ is not available
        // Note: Local queue listening is handled automatically by Wolverine
    }
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5071")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Map Prometheus metrics endpoint
app.MapPrometheusScrapingEndpoint();

app.UseCors();
app.MapHub<LocationHub>("/hubs/locations");

app.Run();