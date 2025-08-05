using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AspireApp.MasterDataService.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogDebug("Cache miss for key: {CacheKey}", key);
                return default;
            }

            _logger.LogDebug("Cache hit for key: {CacheKey}", key);
            return JsonSerializer.Deserialize<T>(cachedData, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from cache for key: {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions();
            
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }

            await _cache.SetStringAsync(key, serialized, options);
            _logger.LogDebug("Cached data for key: {CacheKey} with expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _logger.LogDebug("Removed cache entry for key: {CacheKey}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache for key: {CacheKey}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // Note: Redis doesn't support pattern-based deletion directly in IDistributedCache
        // This would require a more sophisticated implementation with Redis client
        // For now, we'll log this as a limitation
        _logger.LogWarning("Pattern-based cache removal not implemented for pattern: {Pattern}", pattern);
        await Task.CompletedTask;
    }
} 