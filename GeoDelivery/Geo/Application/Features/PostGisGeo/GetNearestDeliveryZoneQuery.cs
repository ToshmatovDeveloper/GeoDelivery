using Geo.Domain.Dtos;
using Geo.Infrastructure.Services;
using MediatR;

namespace Geo.Application.Features;

public record GetNearestDeliveryZoneQuery(
    double Latitude,
    double Longitude) : IRequest<DeliveryZoneDto?>;
    
public record GetNearestDeliveryZoneResponse(DeliveryZoneDto? DeliveryZone);

public class GetNearestDeliveryZoneQueryHandler(
    PostGisGeoService postGisGeoService) : IRequestHandler<GetNearestDeliveryZoneQuery, DeliveryZoneDto?>
{
    public async Task<DeliveryZoneDto?> Handle(GetNearestDeliveryZoneQuery request, CancellationToken cancellationToken)
    {
        return await postGisGeoService.GetNearestDeliveryZoneAsync(
            request.Latitude, 
            request.Longitude, 
            cancellationToken);
    }
}