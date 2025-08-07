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

// Prometheus endpoint temporarily removed

app.UseCors();
app.MapHub<LocationHub>("/hubs/locations");

app.Run();