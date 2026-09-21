using System.Text.Json;
using Catalog.Application.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Caching;

public class RedisCacheService(
    IConnectionMultiplexer redis,
    IOptionsMonitor<CacheSettings> cacheSettings,
    ILogger<RedisCacheService> logger)
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value);
        var timeToLive = expiration ?? cacheSettings.CurrentValue.TimeToLive;
        
        await _db.StringSetAsync(key, json, timeToLive);
        
        logger.LogInformation("Added value to cache with key '{Key}' (TTL: {TTL} minutes)", key, timeToLive.TotalMinutes);
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            logger.LogDebug("Cache miss for key '{Key}'", key);
            return default;
        }
        
        logger.LogInformation("Fetched value from cache for key '{Key}'", key);
        return JsonSerializer.Deserialize<T>(value.ToString()!);
    }
}