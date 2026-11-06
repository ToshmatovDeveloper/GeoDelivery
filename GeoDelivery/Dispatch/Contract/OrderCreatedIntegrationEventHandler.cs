using Dispatch.Domain;
using Dispatch.Infrastructure;
using MediatR;
using Shared.Contracts;

namespace Dispatch.Contract;

public class OrderCreatedIntegrationEventHandler(DispatchDbContext dbContext) 
    : INotificationHandler<OrderCreatedIntegrationEvent>
{
    public async Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var dispatchTask = new DispatchTask(
            notification.OrderId, 
            notification.RestaurantId, 
            notification.Latitude,
            notification.Longitude);

        dbContext.DispatchTasks.Add(dispatchTask);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}