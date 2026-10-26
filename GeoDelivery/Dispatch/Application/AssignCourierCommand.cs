using Dispatch.Domain;
using Dispatch.Infrastructure;
using MediatR;

namespace Dispatch.Application;

public record AssignCourierCommand(Guid OrderId, Guid RestaurantId, Guid CourierId) : IRequest<bool>;

public class AssignCourierCommandHandler(DispatchDbContext dbContext) : IRequestHandler<AssignCourierCommand, bool>
{
    public async Task<bool> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
    {
        var assignment = new CourierAssignment(request.OrderId, request.RestaurantId, request.CourierId);

        dbContext.CourierAssignments.Add(assignment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}