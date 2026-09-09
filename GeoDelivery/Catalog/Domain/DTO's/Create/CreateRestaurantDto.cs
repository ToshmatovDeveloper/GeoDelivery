namespace Catalog.Domain.DTO_s.Create;

public record CreateRestaurantDto(
    string Name,
    string Description,
    bool IsActive);