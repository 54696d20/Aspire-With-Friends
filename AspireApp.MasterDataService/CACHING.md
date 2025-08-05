# Caching Strategy for CQRS

## Overview

This document describes the Redis caching implementation for the CQRS (Command Query Responsibility Segregation) pattern in the MasterDataService.

## Architecture

### Cache Service Layer
- **`ICacheService`**: Interface defining cache operations
- **`RedisCacheService`**: Redis implementation with JSON serialization
- **`BaseHandler`**: Abstract base class providing common caching functionality

### Caching Strategy

#### Query Handlers (Read Operations)
- **Cache-First Approach**: Check cache before hitting database
- **Cache Keys**:
  - `locations_all`: All locations (5-minute TTL)
  - `location_{id}`: Individual location (10-minute TTL)

#### Command Handlers (Write Operations)
- **Cache Invalidation**: Remove related cache entries after data changes
- **Invalidation Strategy**:
  - Create: Invalidate `locations_all`
  - Update: Invalidate `locations_all` and `location_{id}`
  - Delete: Invalidate `locations_all` and `location_{id}`

## Implementation Details

### Cache Service Features
- **Type-Safe Operations**: Generic methods for type-safe caching
- **Error Handling**: Graceful degradation when cache is unavailable
- **Structured Logging**: Detailed cache hit/miss logging
- **JSON Serialization**: Consistent data format with camelCase naming

### Base Handler Methods
```csharp
// Get data from cache
protected async Task<T?> GetFromCacheAsync<T>(string key)

// Set data in cache
protected async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiration = null)

// Invalidate specific cache key
protected async Task InvalidateCacheAsync(string key)

// Invalidate location-related cache
protected async Task InvalidateLocationCacheAsync(int? locationId = null)
```

## Cache Keys

| Key | Description | TTL | Invalidation |
|-----|-------------|-----|--------------|
| `locations_all` | All locations | 5 minutes | On any location change |
| `location_{id}` | Individual location | 10 minutes | On specific location change |

## Benefits

1. **Performance**: Reduced database load for frequently accessed data
2. **Scalability**: Distributed caching across multiple service instances
3. **Consistency**: Proper cache invalidation ensures data consistency
4. **Resilience**: Graceful degradation when cache is unavailable
5. **Observability**: Detailed logging for cache performance monitoring

## Monitoring

### Log Messages
- **Cache Hit**: "Retrieved X locations from cache"
- **Cache Miss**: "Cache miss - retrieving locations from database"
- **Cache Invalidation**: "Invalidated location cache for location ID: X"

### Metrics to Monitor
- Cache hit ratio
- Cache miss frequency
- Cache invalidation events
- Response times (cached vs. non-cached)

## Future Enhancements

1. **Cache Warming**: Pre-populate cache on service startup
2. **Cache Compression**: Compress large datasets
3. **Cache Patterns**: Implement cache-aside and write-through patterns
4. **Cache Clustering**: Redis cluster for high availability
5. **Cache Analytics**: Detailed cache performance analytics 