using Catalog.Application.Caching;
using Catalog.Application.CustomExceptions;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Restaurant.Queries;

public record GetRestaurantByNameQuery(string Name) : IRequest<GetRestaurantByNameResponse>, ICachableQuery
{
    public string CacheKey => $"restaurant:name:{Name.ToLowerInvariant()}";
}

public record GetRestaurantByNameResponse(RestaurantDto? Dto, string Message);

public class GetRestaurantByNameQueryHandler(
    CatalogDbContext dbContext,
    ILogger<GetRestaurantByNameQueryHandler> logger) : IRequestHandler<GetRestaurantByNameQuery, GetRestaurantByNameResponse>
{
    public async Task<GetRestaurantByNameResponse> Handle(GetRestaurantByNameQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching restaurant {Name}...", query.Name);

        var restaurant = await dbContext.Restaurants
            .FirstOrDefaultAsync(r => r.Name == query.Name, cancellationToken);

        if (restaurant is null)
        {
            logger.LogInformation("Restaurant {Name} not found.", query.Name);
            
            throw new NotFoundException($"Restaurant {query.Name} not found.");
        }

        var dto = new RestaurantDto(restaurant.Id, restaurant.Name, restaurant.Description, restaurant.IsActive);

        logger.LogInformation("Restaurant {Name} fetched successfully.", query.Name);
        
        return new GetRestaurantByNameResponse(dto, "Restaurant fetched successfully.");
    }
}