namespace Order.Application.Dtos;

public record OrderItemResponseDto(
    Guid DishId,
    string DishName,
    int Quantity,
    decimal Price,
    decimal TotalPrice);