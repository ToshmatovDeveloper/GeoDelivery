namespace Order.Application.Dtos;

public record OrderItemResponseDto(Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);