using Catalog.Application.Caching;
using Catalog.Application.CustomExceptions;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;

namespace Catalog.Application.Features.Restaurant.Queries;

public record GetRestaurantByIdQuery(Guid RestaurantId) : IRequest<GetRestaurantByIdResponse>, ICachableQuery
{
    public string CacheKey => $"restaurant:{RestaurantId}";
}

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
            throw new NotFoundException($"Restaurant {query.RestaurantId} not found.");
        }

        var dto = new RestaurantDto(restaurant.Id, restaurant.Name, restaurant.Description, restaurant.IsActive);

        return new GetRestaurantByIdResponse(dto, "Restaurant fetched successfully.");
    }
}