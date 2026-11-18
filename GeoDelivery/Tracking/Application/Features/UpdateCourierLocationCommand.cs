using MediatR;
using Tracking.Domain.Entities;
using Tracking.Infrastructure;

namespace Tracking.Application.Features;

public record UpdateCourierLocationCommand(
    Guid CourierId,
    Guid? OrderId,
    double Latitude,
    double Longitude) : IRequest<Guid>;
    
public class UpdateCourierLocationHandler(
    TrackingDbContext dbContext) : IRequestHandler<UpdateCourierLocationCommand, Guid>
{
    public async Task<Guid> Handle(UpdateCourierLocationCommand request, CancellationToken cancellationToken)
    {
        var locationPoint = new CourierLocationPoint(
            request.CourierId,
            request.OrderId,
            request.Latitude,
            request.Longitude);

        dbContext.CourierLocationPoints.Add(locationPoint);
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return locationPoint.Id;
    }
}