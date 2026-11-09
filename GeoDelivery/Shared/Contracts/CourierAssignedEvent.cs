using MediatR;

namespace Shared.Contracts;

public record CourierAssignedEvent(
    Guid OrderId,
    Guid CourierId,
    Guid RestaurantId,
    DateTime AssignedAt
) : INotification;