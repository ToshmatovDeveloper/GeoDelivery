using Geo.Infrastructure.Services;
using MediatR;

namespace Geo.Application.Features;

public record CalculateDistanceQuery(
    double Latitude1,
    double Longitude1,
    double Latitude2,
    double Longitude2) : IRequest<CalculateDistanceResponse>;

public record CalculateDistanceResponse(double DistanceInMeters);

public class CalculateDistanceQueryHandler(
    PostGisGeoService postGisGeoService) : IRequestHandler<CalculateDistanceQuery, CalculateDistanceResponse>
{
    public Task<CalculateDistanceResponse> Handle(CalculateDistanceQuery request, CancellationToken cancellationToken)
    {
        var distance = postGisGeoService.CalculateDistanceInMeters(
            request.Latitude1, 
            request.Longitude1, 
            request.Latitude2, 
            request.Longitude2);
        
        return Task.FromResult(new CalculateDistanceResponse(distance));
    }
}