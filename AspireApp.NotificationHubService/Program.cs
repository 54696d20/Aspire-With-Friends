using AspireApp.NotificationHubService.Hubs;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR
builder.Services.AddSignalR();

// Add Wolverine with RabbitMQ
builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbitMqConnectionString: builder.Configuration.GetConnectionString("rabbitmq"))
        .AutoPurgeOnStartup()
        .AutoProvision();
    
    opts.ListenToRabbitQueue("wolverine");
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
app.UseCors();
app.MapHub<LocationHub>("/hubs/locations");

app.Run();