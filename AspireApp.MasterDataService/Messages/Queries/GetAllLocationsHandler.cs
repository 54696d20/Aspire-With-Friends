using AspireApp.MasterDataService.Data;
using AspireApp.MasterDataService.Models;
using AspireApp.MasterDataService.Handlers;
using AspireApp.MasterDataService.Services;
using Dapper;
using Microsoft.Extensions.Logging;

namespace AspireApp.MasterDataService.Messages.Queries;

public class GetAllLocationsHandler : BaseHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetAllLocationsHandler(IDbConnectionFactory connectionFactory, ICacheService cache, ILogger<GetAllLocationsHandler> logger)
        : base(cache, logger)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Location>> Handle(GetAllLocationsQuery query)
    {
        const string cacheKey = "locations_all";
        
        // Try to get from cache first
        var cachedLocations = await GetFromCacheAsync<IEnumerable<Location>>(cacheKey);
        if (cachedLocations != null)
        {
            _logger.LogInformation("Retrieved {LocationCount} locations from cache", cachedLocations.Count());
            return cachedLocations;
        }

        // If not in cache, get from database
        _logger.LogInformation("Cache miss - retrieving locations from database");
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var locations = (await connection.QueryAsync<Location>("SELECT * FROM Locations")).ToList();

        // Cache the result for 5 minutes
        await SetCacheAsync(cacheKey, locations, TimeSpan.FromMinutes(5));
        _logger.LogInformation("Cached {LocationCount} locations", locations.Count);
        
        return locations;
    }
} 