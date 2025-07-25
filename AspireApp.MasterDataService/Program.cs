using AspireApp.MasterDataService.Data;
using Wolverine;
using Wolverine.RabbitMQ;
using AspireApp.Shared.Messaging;
using AspireApp.Shared.Messaging.Models;

var builder = WebApplication.CreateBuilder(args);
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

});

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();