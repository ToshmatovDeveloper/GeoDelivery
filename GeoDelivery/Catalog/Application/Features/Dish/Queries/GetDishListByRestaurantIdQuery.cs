using Catalog.Application.Caching;
using Catalog.Application.CustomExceptions;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Dish.Queries;

public record GetDishListByRestaurantIdQuery(Guid RestaurantId) : IRequest<GetDishListByRestaurantIdResponse>, ICachableQuery
{
    public string CacheKey => $"dishes:restaurant:{RestaurantId}";
}

public record GetDishListByRestaurantIdResponse(IEnumerable<DishDto>? Dishes, string Message);

public class GetDishesByRestaurantIdQueryHandler(
    CatalogDbContext dbContext,
    ILogger<GetDishesByRestaurantIdQueryHandler> logger) : IRequestHandler<GetDishListByRestaurantIdQuery, GetDishListByRestaurantIdResponse>
{
    public async Task<GetDishListByRestaurantIdResponse> Handle(GetDishListByRestaurantIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching dishes for restaurant {RestaurantId}...", request.RestaurantId);
        
        var dishes = await dbContext.Dishes
            .Where(d => d.RestaurantId == request.RestaurantId)
            .Select(d => new DishDto(
                d.Id,
                d.RestaurantId,
                d.CategoryId,
                d.Name,
                d.Price,
                d.IsAvailable))
            .ToListAsync(cancellationToken);
        
        if (!dishes.Any())
        {
            logger.LogInformation("No dishes found.");
            throw new NotFoundException("No dishes found.");
        }

        logger.LogInformation("Dishes found.");
        
        return new GetDishListByRestaurantIdResponse(dishes, "Dishes fetched successfully.");
    }
}