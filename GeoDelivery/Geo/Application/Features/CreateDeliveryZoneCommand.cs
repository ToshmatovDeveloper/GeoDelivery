using Geo.Domain;
using Geo.Domain.Dtos;
using Geo.Infrastructure;
using MediatR;
using NetTopologySuite.Geometries;

namespace Geo.Application.Features;

public record CreateDeliveryZoneCommand(
    string Name,
    List<LocationPointDto> Points,
    bool IsActive ) : IRequest<CreateDeliveryZoneResponse>;
    
public record CreateDeliveryZoneResponse(Guid Id, string Message);    

public class CreateDeliveryZoneCommandHandler(
    GeoDbContext dbContext,
    GeometryFactory geometryFactory) : IRequestHandler<CreateDeliveryZoneCommand, CreateDeliveryZoneResponse>
{
    public async Task<CreateDeliveryZoneResponse> Handle(CreateDeliveryZoneCommand request, CancellationToken cancellationToken)
    {
        var pointList = request.Points
            .Select(p => new Coordinate(p.Longitude, p.Latitude))
            .ToList();

        if (pointList.Count > 0 && !pointList[0].Equals(pointList[^1]))
        {
            pointList.Add(pointList[0]);
        }
        
        var coordinates = pointList.ToArray();
        
        var linearRing = geometryFactory.CreateLinearRing(coordinates);
        var polygon = geometryFactory.CreatePolygon(linearRing);
        polygon.SRID = 4326;

        var deliveryZone = new Domain.DeliveryZone(
            restaurantId: Guid.NewGuid(),
            name: request.Name,
            boundary: polygon,
            deliveryCost: 0m
        );
        
        dbContext.DeliveryZones.Add(deliveryZone);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new CreateDeliveryZoneResponse(
            deliveryZone.Id, $"Delivery zone '{deliveryZone.Name}' successfully created.");
    }
}   