using AspireApp.MasterDataService.Services;
using Microsoft.Extensions.Logging;

namespace AspireApp.MasterDataService.Handlers;

public abstract class BaseHandler
{
    protected readonly ICacheService _cache;
    protected readonly ILogger _logger;

    protected BaseHandler(ICacheService cache, ILogger logger)
    {
        _cache = cache;
        _logger = logger;
    }

    protected async Task<T?> GetFromCacheAsync<T>(string key)
    {
        return await _cache.GetAsync<T>(key);
    }

    protected async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        await _cache.SetAsync(key, value, expiration);
    }

    protected async Task InvalidateCacheAsync(string key)
    {
        await _cache.RemoveAsync(key);
        _logger.LogInformation("Invalidated cache for key: {CacheKey}", key);
    }

    protected async Task InvalidateLocationCacheAsync(int? locationId = null)
    {
        // Always invalidate the "all locations" cache
        await _cache.RemoveAsync("locations_all");
        
        // If a specific location ID is provided, invalidate that too
        if (locationId.HasValue)
        {
            await _cache.RemoveAsync($"location_{locationId.Value}");
        }
        
        _logger.LogInformation("Invalidated location cache for location ID: {LocationId}", locationId);
    }
} 