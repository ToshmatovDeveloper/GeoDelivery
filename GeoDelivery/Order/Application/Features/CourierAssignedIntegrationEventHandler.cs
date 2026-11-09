using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Enums;
using Order.Infrastructure;
using Shared.Contracts;

namespace Order.Application.Features;

public class CourierAssignedIntegrationEventHandler(OrderDbContext dbContext)
    : INotificationHandler<CourierAssignedEvent>
{
    public async Task Handle(CourierAssignedEvent notification, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .SingleOrDefaultAsync(order => order.Id == notification.OrderId, cancellationToken);

        if (order is null)
        {
            throw new InvalidOperationException($"Order {notification.OrderId} was not found.");
        }

        order.CourierId = notification.CourierId;
        order.Status = OrderStatus.Delivering;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
