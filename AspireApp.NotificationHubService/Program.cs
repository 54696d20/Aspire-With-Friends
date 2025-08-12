using AspireApp.NotificationHubService.Hubs;
using Wolverine;
using Wolverine.RabbitMQ;
// OpenTelemetry using statements removed

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry temporarily removed for simple working state

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

// Add CORS
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

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), ["live"]);

var app = builder.Build();

// Prometheus endpoint temporarily removed

app.UseCors();

// Map health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

app.MapHub<LocationHub>("/hubs/locations");

app.Run();