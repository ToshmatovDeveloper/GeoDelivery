namespace Geo.Domain.Dtos;

public record DeliveryZoneDto(
    Guid Id,
    string Name,
    bool IsActive
);