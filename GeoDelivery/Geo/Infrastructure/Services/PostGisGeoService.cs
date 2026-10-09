using Geo.Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Geo.Infrastructure.Services;

public class PostGisGeoService(
    GeoDbContext geoDbContext,
    GeometryFactory geometryFactory) 
{
    public async Task<bool> IsPointInZoneAsync(
        Guid zoneId, 
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var point = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        
        return await geoDbContext.DeliveryZones
            .AsNoTracking()
            .Where(zone => zone.Id == zoneId && zone.IsActive)
            .AnyAsync(zone => zone.Boundary.Contains(point), cancellationToken);
    }

    public double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        var point1 = geometryFactory.CreatePoint(new Coordinate(lon1, lat1));
        var point2 = geometryFactory.CreatePoint(new Coordinate(lon2, lat2));

        return point1.Distance(point2) * 111_139;
    }

    public async Task<DeliveryZoneDto?> GetNearestDeliveryZoneAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var point = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        
        var zone = await geoDbContext.DeliveryZones
            .AsNoTracking()
            .Where(z => z.IsActive)
            .OrderBy(z => z.Boundary.Distance(point))
            .FirstOrDefaultAsync(cancellationToken);

        if (zone == null)
            return null;

        return new DeliveryZoneDto(
            Id: zone.Id,
            Name: zone.Name,
            IsActive: zone.IsActive
        );           
    }
}