using StackExchange.Redis;

namespace Geo.Infrastructure.Services;

public class RedisGeoService(IConnectionMultiplexer redis) 
{
    public async Task UpdateCourierLocationAync(string geoKey, Guid courierId, double latitude, double longitude)
    {
        var db = redis.GetDatabase();
        
        await db.GeoAddAsync(geoKey, longitude, latitude, courierId.ToString());
    }

    public async Task<List<Guid>> GetCouriersWithinRadiusAsync(
        string geoKey,
        double latitude,
        double longitude,
        double radiusInKm)
    {
        var db = redis.GetDatabase();
        
        var results = await db.GeoSearchAsync(
            key: geoKey,
            longitude: longitude,
            latitude: latitude,
            shape: new GeoSearchCircle(radiusInKm, GeoUnit.Kilometers)
        );

        return results
            .Select(r => Guid.Parse(r.Member.ToString())) 
            .ToList();
    }
    
    public async Task RemoveCourierAsync(string geoKey, Guid courierId)
    {
        var db = redis.GetDatabase();
        await db.SortedSetRemoveAsync(geoKey, courierId.ToString());
    }
}