using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Models;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AspireApp.MasterDataService.Messages.Queries;

public class GetLocationByIdHandler
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetLocationByIdHandler> _logger;

    public GetLocationByIdHandler(IDbConnectionFactory connectionFactory, IDistributedCache cache, ILogger<GetLocationByIdHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Location?> Handle(GetLocationByIdQuery query)
    {
        var cacheKey = $"location_{query.Id}";
        
        // Try to get from cache first
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Retrieved location {LocationId} from cache", query.Id);
            return JsonSerializer.Deserialize<Location>(cachedData);
        }

        // If not in cache, get from database
        _logger.LogInformation("Cache miss - retrieving location {LocationId} from database", query.Id);
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var location = await connection.QueryFirstOrDefaultAsync<Location>(
            "SELECT * FROM Locations WHERE Id = @Id", new { query.Id });

        if (location != null)
        {
            // Cache the result
            var serialized = JsonSerializer.Serialize(location);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
            });
            _logger.LogInformation("Cached location {LocationId}", query.Id);
        }

        return location;
    }
} 