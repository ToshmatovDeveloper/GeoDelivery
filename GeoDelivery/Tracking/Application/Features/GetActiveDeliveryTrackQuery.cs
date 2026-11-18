using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracking.Application.Features.DTOs;
using Tracking.Infrastructure;

namespace Tracking.Application.Features;

public record GetActiveDeliveryTrackQuery(Guid OrderId) : IRequest<DeliveryTrackDto?>;

public class GetActiveDeliveryTrackHandler(
    TrackingDbContext dbContext) : IRequestHandler<GetActiveDeliveryTrackQuery, DeliveryTrackDto?>
{
    public async Task<DeliveryTrackDto?> Handle(GetActiveDeliveryTrackQuery request, CancellationToken cancellationToken)
    {
        var points = await dbContext.CourierLocationPoints
            .AsNoTracking()
            .Where(x => x.OrderId == request.OrderId)
            .OrderBy(x => x.Timestamp)
            .ToListAsync(cancellationToken);

        if (points.Count == 0)
        {
            return null;
        }

        var routeHistory = points
            .Select(p => new LocationPointDto(p.Latitude, p.Longitude, p.Timestamp))
            .ToList();

        var latestPoint = routeHistory[^1];
        
        var courierId = points[^1].CourierId;

        return new DeliveryTrackDto(
            OrderId: request.OrderId,
            CourierId: courierId,
            CurrentLocation: latestPoint,
            RouteHistory: routeHistory);
    }
}