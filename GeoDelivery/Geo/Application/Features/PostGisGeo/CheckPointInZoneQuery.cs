using Geo.Infrastructure.Services;
using MediatR;

namespace Geo.Application.Features;

public record CheckPointInZoneQuery(
    Guid ZoneId,
    double Latitude,
    double Longitude) : IRequest<CheckPointInZoneRespone>;
    
public record  CheckPointInZoneRespone(bool IsInsideZone);

public class CheckPointInZoneQueryHandler(
    PostGisGeoService postGisGeoService) : IRequestHandler<CheckPointInZoneQuery, CheckPointInZoneRespone>
{
    public async Task<CheckPointInZoneRespone> Handle(CheckPointInZoneQuery request, CancellationToken cancellationToken)
    {
        var isInZone = await postGisGeoService.IsPointInZoneAsync(
            request.ZoneId, 
            request.Latitude, 
            request.Longitude, 
            cancellationToken);
        
        return new CheckPointInZoneRespone(isInZone);
    }
}