using MediatR;
using Order.Domain.Enums;
using Order.Infrastructure;

namespace Order.Application.Features;

public record CancelOrderCommand(Guid OrderId, Guid UserId) : IRequest<bool>;

public class CancelOrderCommandHandler(OrderDbContext dbContext) 
    : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
        
        if (order == null || order.CustomerId != request.UserId)
        {
            return false;
        }

        order.Status = OrderStatus.Cancelled;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}