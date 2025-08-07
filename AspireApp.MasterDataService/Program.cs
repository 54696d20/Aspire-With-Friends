using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Services;
using Wolverine;
using Wolverine.RabbitMQ;
using AspireApp.Shared.Messaging;
using AspireApp.Shared.Messaging.Models;
using FluentValidation;
using AspireApp.MasterDataService.Validators;
using Wolverine.FluentValidation;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "MasterDataService", serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
        .AddRedisInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

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
    opts.UseRabbitMq(rabbitMqConnectionString: builder.Configuration.GetConnectionString("rabbitmq"))
        .AutoPurgeOnStartup()
        .AutoProvision();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.PublishMessage<LocationChangedNotificationModel>()
        .ToRabbitQueue("wolverine");

    // Enable FluentValidation
    opts.UseFluentValidation();
});

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateLocationCommandValidator>();

builder.Services.AddControllers();

var app = builder.Build();

// Map Prometheus metrics endpoint
app.MapPrometheusScrapingEndpoint();

app.MapControllers();
app.Run();