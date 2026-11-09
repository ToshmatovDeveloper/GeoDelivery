using MediatR;
using Shared.Contracts; 

namespace Dispatch.Application;

public record FindOptimalCourierQuery(Guid RestaurantId, double Latitude, double Longitude) : IRequest<Guid?>;

public class FindOptimalCourierQueryHandler(
    IMediator mediator) : IRequestHandler<FindOptimalCourierQuery, Guid?>
{
    public async Task<Guid?> Handle(FindOptimalCourierQuery request, CancellationToken cancellationToken)
    {
        var courierIds = await mediator.Send(
            new GetNearbyCouriersQuery(request.Latitude, request.Longitude, RadiusInKm: 5.0), 
            cancellationToken);

        var bestCourierId = courierIds.FirstOrDefault();

        return bestCourierId == default ? null : bestCourierId;
    }
}