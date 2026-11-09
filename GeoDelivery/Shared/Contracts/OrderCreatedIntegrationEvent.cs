using MediatR;

namespace Shared.Contracts;

public record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid RestaurantId,
    double Latitude,
    double Longitude,
    DateTime CreatedAt
) : INotification;