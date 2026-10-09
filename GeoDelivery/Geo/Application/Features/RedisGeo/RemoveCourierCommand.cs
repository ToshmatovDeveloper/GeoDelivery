using Geo.Infrastructure.Services;
using MediatR;

namespace Geo.Application.Features.RedisGeo;

public record RemoveCourierCommand(Guid CourierId) : IRequest;

public class RemoveCourierCommandHandler(
    RedisGeoService geoService) : IRequestHandler<RemoveCourierCommand>
{
    private const string ActiveCouriersKey = "active_couriers";
    
    public async Task Handle(RemoveCourierCommand request, CancellationToken cancellationToken)
    {
        await geoService.RemoveCourierAsync(ActiveCouriersKey, request.CourierId);
    }
}