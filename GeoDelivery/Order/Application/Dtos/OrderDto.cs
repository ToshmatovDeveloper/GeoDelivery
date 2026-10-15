namespace Order.Application.Dtos;

public record OrderDto(Guid Id,
    Guid RestaurantId,
    Guid CustomerId,
    string Status,
    double DeliveryLatitude,
    double DeliveryLongitude,
    decimal TotalPrice,
    DateTimeOffset CreatedAt,
    List<OrderItemResponseDto> Items);