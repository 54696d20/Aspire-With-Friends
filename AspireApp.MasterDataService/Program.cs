using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
var connectionString = builder.Configuration.GetConnectionString("masterdatadb");
//Console.WriteLine($"[DEBUG] ConnectionString: {connectionString}");

builder.Services.AddScoped<LocationRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

var app = builder.Build();

// Minimal API Endpoints for Locations
app.MapGet("/locations", async (LocationRepository repo) =>
{
    var locations = await repo.GetAllAsync();
    return Results.Ok(locations);
});

app.MapGet("/locations/{id}", async (LocationRepository repo, int id) =>
{
    var location = await repo.GetByIdAsync(id);
    
    return location is null ? Results.NotFound() : Results.Ok(location);
});

app.MapPost("/locations", async (LocationRepository repo, Location location) =>
{
    await repo.AddAsync(location);
    
    return Results.Created($"/locations/{location.Id}", location);
});

app.Run();