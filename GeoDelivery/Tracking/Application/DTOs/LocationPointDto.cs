namespace Tracking.Application.Features.DTOs;

public record LocationPointDto(
    double Latitude,
    double Longitude,
    DateTime Timestamp);