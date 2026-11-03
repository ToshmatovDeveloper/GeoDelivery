using Dispatch.Domain;
using Dispatch.Infrastructure;
using MediatR;

namespace Dispatch.Contract;

public record OrderCreatedIntegrationEvent(
    Guid OrderId, 
    Guid RestaurantId, 
    double Latitude, 
    double Longitude) : INotification;

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