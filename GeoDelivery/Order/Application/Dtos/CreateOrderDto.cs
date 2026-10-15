namespace Order.Application.Dtos;

public record CreateOrderDto(Guid RestaurantId,
    Guid CustomerId,
    double DeliveryLatitude,
    double DeliveryLongitude,
    List<OrderItemDto> Items);