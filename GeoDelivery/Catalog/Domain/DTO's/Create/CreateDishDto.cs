namespace Catalog.Domain.DTO_s.Create;

public record CreateDishDto(
    Guid RestaurantId,
    Guid CategoryId,
    string Name,
    decimal Price,
    bool IsAvailable);