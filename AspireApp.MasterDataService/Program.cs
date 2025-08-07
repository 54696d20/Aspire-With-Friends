using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Services;
using Wolverine;
using Wolverine.RabbitMQ;
using AspireApp.Shared.Messaging;
using AspireApp.Shared.Messaging.Models;
using FluentValidation;
using AspireApp.MasterDataService.Validators;
using Wolverine.FluentValidation;
// OpenTelemetry using statements removed

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry temporarily removed for simple working state

//Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var logger = builder.Logging.Services.BuildServiceProvider()
    .GetRequiredService<ILogger<Program>>();
//logger.LogInformation("✅ MasterDataService is starting up...");

// Add services
//builder.Configuration.GetConnectionString("masterdatadb");
//builder.Services.AddScoped<LocationRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Add cache service
builder.Services.AddScoped<ICacheService, RedisCacheService>();

//Add Wolverine
builder.Services.AddWolverine(opts =>
{
    var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq");
    if (!string.IsNullOrEmpty(rabbitMqConnectionString))
    {
        opts.UseRabbitMq(rabbitMqConnectionString: rabbitMqConnectionString)
            .AutoPurgeOnStartup()
            .AutoProvision();
        
        opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
        opts.Policies.UseDurableInboxOnAllListeners();
        opts.PublishMessage<LocationChangedNotificationModel>()
            .ToRabbitQueue("wolverine");
    }
    else
    {
        // Fallback to local queue if RabbitMQ is not available
        opts.PublishMessage<LocationChangedNotificationModel>()
            .ToLocalQueue("wolverine");
    }

    // Enable FluentValidation
    opts.UseFluentValidation();
});

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateLocationCommandValidator>();

builder.Services.AddControllers();

var app = builder.Build();

// Prometheus endpoint temporarily removed

app.MapControllers();
app.Run();