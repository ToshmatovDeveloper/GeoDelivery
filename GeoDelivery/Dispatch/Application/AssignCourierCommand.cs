using Dispatch.Domain;
using Dispatch.Infrastructure;
using MediatR;
using Shared.Contracts;

namespace Dispatch.Application;

public record AssignCourierCommand(Guid OrderId, Guid RestaurantId, Guid CourierId) : IRequest<bool>;

public class AssignCourierCommandHandler(DispatchDbContext dbContext, IMediator mediator) : IRequestHandler<AssignCourierCommand, bool>
{
    public async Task<bool> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
    {
        var assignment = new CourierAssignment(request.OrderId, request.RestaurantId, request.CourierId);

        dbContext.CourierAssignments.Add(assignment);
        await dbContext.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new CourierAssignedEvent(
            request.OrderId,
            request.CourierId,
            request.RestaurantId,
            assignment.AssignedAt
        ), cancellationToken);

        return true;
    }
}