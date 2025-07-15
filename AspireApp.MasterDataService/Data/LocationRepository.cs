using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Dapper;
using AspireApp.MasterDataService.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace AspireApp.MasterDataService.Data;

public class LocationRepository
{
    private readonly string _connectionString;
    private readonly IDistributedCache _cache;

    public LocationRepository(IConfiguration config, IDistributedCache cache)
    {
        _connectionString = config.GetConnectionString("masterdatadb")!;
        _cache = cache;
    }

    private IDbConnection Connection => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        const string cacheKey = "locations_all";

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<Location>>(cachedData) ?? new List<Location>();
        }

        const string sql = "SELECT * FROM Locations";
        using var db = Connection;
        var locations = (await db.QueryAsync<Location>(sql)).ToList();

        var serialized = JsonSerializer.Serialize(locations);
        await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        });

        return locations;
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Locations WHERE Id = @Id";
        using var db = Connection;
        
        await _cache.RemoveAsync("locations_all");
        
        return await db.QuerySingleOrDefaultAsync<Location>(sql, new { Id = id });
    }

    public async Task AddAsync(Location location)
    {
        const string sql = "INSERT INTO Locations (Name, Type, ParentId) VALUES (@Name, @Type, @ParentId)";
        using var db = Connection;
        
        await _cache.RemoveAsync("locations_all");
        
        await db.ExecuteAsync(sql, location);
    }
    
    public async Task UpdateAsync(Location location)
    {
        const string sql = "UPDATE Locations SET Name = @Name, Description = @Description WHERE Id = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, location);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Locations WHERE Id = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, new { Id = id });
    }

}