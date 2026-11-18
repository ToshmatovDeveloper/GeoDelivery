namespace Tracking.Application.Features.DTOs;

public record DeliveryTrackDto(
    Guid OrderId,
    Guid CourierId,
    LocationPointDto? CurrentLocation,
    IReadOnlyList<LocationPointDto> RouteHistory);