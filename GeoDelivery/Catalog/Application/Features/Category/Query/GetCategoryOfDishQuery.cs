using Catalog.Application.Caching;
using Catalog.Application.CustomExceptions;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Category.Query;

public record GetCategoryOfDishQuery(Guid DishId, Guid RestaurantId) 
    : IRequest<GetCategoryOfDishResponse>, ICachableQuery
{
    public string CacheKey => $"category:restaurant:{RestaurantId}:dish:{DishId}";
}

public record GetCategoryOfDishResponse(CategoryDto? Category, string Message);

public class GetCategoryOfDishQueryHandler(
    CatalogDbContext catalogDbContext, 
    ILogger<GetCategoryOfDishQueryHandler> logger) : IRequestHandler<GetCategoryOfDishQuery, GetCategoryOfDishResponse>
{
    public async Task<GetCategoryOfDishResponse> Handle(GetCategoryOfDishQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching category for dish {DishId} in restaurant {RestaurantId}", request.DishId, request.RestaurantId);
        
        var category = await catalogDbContext.Categories
            .Where(c => c.RestaurantId == request.RestaurantId)
            .Join(catalogDbContext.Dishes,
                category => category.Id,
                dish => dish.CategoryId,
                (category, dish) => new { Category = category, Dish = dish })
            .Where(cd => cd.Dish.Id == request.DishId)
            .Select(cd => new CategoryDto(cd.Category.Id, cd.Category.RestaurantId, cd.Category.Name))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (category is null)
        {
            logger.LogWarning("Category for dish {DishId} in restaurant {RestaurantId} not found", request.DishId, request.RestaurantId);
            throw new NotFoundException($"Category for dish with ID {request.DishId} in restaurant {request.RestaurantId} was not found.");
        }
        
        return new GetCategoryOfDishResponse(category, "Category fetched successfully.");
    }
}