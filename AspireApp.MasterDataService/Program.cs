using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Interfaces;
using AspireApp.MasterDataService.Models;
using AspireApp.MasterDataService.Services;
using AspireApp.MasterDataService.Data;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Configuration.GetConnectionString("masterdatadb");

builder.Services.AddScoped<LocationRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

//Add Wolverine
builder.Services.AddWolverine(opts =>
{
    opts.UseRabbitMq(builder.Configuration.GetConnectionString("RabbitMQ"))
        .AutoProvision();
});

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();