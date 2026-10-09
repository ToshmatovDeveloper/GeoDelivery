using Geo.Infrastructure.Services;
using MediatR;

namespace Geo.Application.Features.RedisGeo;

public record UpdateCourierLocationCommand(
    Guid CourierId,
    double Latitude,
    double Longitude) : IRequest;
    
public class UpdateCourierLocationCommandHandler(
    RedisGeoService geoService) : IRequestHandler<UpdateCourierLocationCommand>
{
    private const string ActiveCouriersKey = "active_couriers";
    
    public async Task Handle(UpdateCourierLocationCommand request, CancellationToken cancellationToken)
    { 
        await geoService.UpdateCourierLocationAync(
            ActiveCouriersKey, 
            request.CourierId, 
            request.Latitude, 
            request.Longitude);
    }
}