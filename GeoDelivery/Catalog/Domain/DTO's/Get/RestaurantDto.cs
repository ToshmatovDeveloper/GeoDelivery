namespace Catalog.Domain.DTO_s.Get;

public record RestaurantDto(
    Guid RestaurantId,
    string Name,
    string Description,
    bool IsActive);