using MediatR;

namespace Dispatch.Application;

public record FindOptimalCourierQuery(Guid RestaurantId, double Latitude, double Longitude) : IRequest<Guid?>;

public class FindOptimalCourierQueryHandler : IRequestHandler<FindOptimalCourierQuery, Guid?>
{
    public async Task<Guid?> Handle(FindOptimalCourierQuery request, CancellationToken cancellationToken)
    {
        // TODO: Integrate with the Geo module (PostGIS / Redis GEO) to search for available couriers within the delivery radius.
        await Task.CompletedTask;
        
        return Guid.NewGuid();
    }
}