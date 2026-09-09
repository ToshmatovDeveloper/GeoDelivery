using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Restaurant.Queries;

public record GetRestaurantByIdQuery(Guid RestaurantId) : IRequest<GetRestaurantByIdResponse>;

public record GetRestaurantByIdResponse(RestaurantDto? Dto, string Message);

public class GetRestaurantByIdQueryHandler(
    CatalogDbContext dbContext,
    ILogger<GetRestaurantByIdQueryHandler> logger) : IRequestHandler<GetRestaurantByIdQuery, GetRestaurantByIdResponse>
{
    public async Task<GetRestaurantByIdResponse> Handle(GetRestaurantByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching restaurant {RestaurantId}...", query.RestaurantId);

        var restaurant = await dbContext.Restaurants.FindAsync([query.RestaurantId], cancellationToken);

        if (restaurant is null)
        {
            return new GetRestaurantByIdResponse(null, "Restaurant not found.");
        }

        var dto = new RestaurantDto(restaurant.Id, restaurant.Name, restaurant.Description, restaurant.IsActive);

        return new GetRestaurantByIdResponse(dto, "Restaurant fetched successfully.");
    }
}