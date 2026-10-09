using Geo.Infrastructure.Services;
using MediatR;

namespace Geo.Application.Features.RedisGeo;

public record GetNearbyCouriersQuery(
    double Latitude,
    double Longitude,
    double RadiusInKm) : IRequest<List<Guid>>;
    
public class GetNearbyCouriersQueryHandler(
    RedisGeoService geoService) : IRequestHandler<GetNearbyCouriersQuery, List<Guid>>
{
    public Task<List<Guid>> Handle(GetNearbyCouriersQuery request, CancellationToken cancellationToken)
    {
        return geoService.GetCouriersWithinRadiusAsync(
            "active_couriers", 
            request.Latitude, 
            request.Longitude, 
            request.RadiusInKm);
    }
}