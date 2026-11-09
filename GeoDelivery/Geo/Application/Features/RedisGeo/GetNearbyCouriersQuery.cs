using Geo.Infrastructure.Services;
using MediatR;
using Shared.Contracts; 

namespace Geo.Application.Features.RedisGeo;

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