using MediatR;

namespace Shared.Contracts;

public record GetNearbyCouriersQuery(
    double Latitude,
    double Longitude,
    double RadiusInKm) : IRequest<List<Guid>>;