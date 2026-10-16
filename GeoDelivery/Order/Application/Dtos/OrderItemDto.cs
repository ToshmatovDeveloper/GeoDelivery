namespace Order.Application.Dtos;

public record OrderItemDto(
    Guid DishId,
    string DishName,
    int Quantity,
    decimal Price);