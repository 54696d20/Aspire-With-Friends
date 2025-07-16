using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Interfaces;
using AspireApp.MasterDataService.Models;
using AspireApp.MasterDataService.Services;
using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Messages;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
//Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var logger = builder.Logging.Services.BuildServiceProvider()
    .GetRequiredService<ILogger<Program>>();
logger.LogInformation("✅ MasterDataService is starting up...");

// Add services
//builder.Configuration.GetConnectionString("masterdatadb");
//builder.Services.AddScoped<LocationRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

//Add Wolverine
builder.Services.AddWolverine(opts =>
{
    opts.UseRabbitMq(rabbitMqConnectionString: builder.Configuration.GetConnectionString("RabbitMQ"))
        .AutoPurgeOnStartup()
        .AutoProvision();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.PublishMessage<LocationChangedNotification>()
        .ToRabbitQueue("wolverine");

});

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();