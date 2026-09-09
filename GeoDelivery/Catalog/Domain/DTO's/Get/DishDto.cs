namespace Catalog.Domain.DTO_s.Get;

public record DishDto(
    Guid DishId,
    Guid RestaurantId,
    Guid CategoryId,
    string Name,
    decimal Price,
    bool IsAvailable);