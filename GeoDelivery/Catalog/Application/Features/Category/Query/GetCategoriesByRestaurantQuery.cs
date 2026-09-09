using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Category.Query;

public record GetCategoriesByRestaurantQuery(Guid RestaurantId) : IRequest<GetCategoriesByRestaurantResponse>;
public record GetCategoriesByRestaurantResponse(IEnumerable<CategoryDto>? Categories, string Message);

public class GetCategoriesByRestaurantQueryHandler(
    CatalogDbContext context, 
    ILogger<GetCategoriesByRestaurantQueryHandler> logger) : IRequestHandler<GetCategoriesByRestaurantQuery, GetCategoriesByRestaurantResponse>
{
    public async Task<GetCategoriesByRestaurantResponse> Handle(GetCategoriesByRestaurantQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Fetching categories for restaurant {request.RestaurantId}");
        
        var restaurantCheck = await context.Restaurants.FindAsync(request.RestaurantId);
        
        if (restaurantCheck is null)
        {
            logger.LogWarning($"Restaurant with ID {request.RestaurantId} not found.");
            
            return new GetCategoriesByRestaurantResponse(null, $"Restaurant with ID {request.RestaurantId} not found.");
        }
        
        var categories = await context.Categories
            .Where(c => c.RestaurantId == request.RestaurantId)
            .Select(c => new CategoryDto(c.Id, c.RestaurantId, c.Name))
            .ToListAsync(cancellationToken);

        return new GetCategoriesByRestaurantResponse(categories, "Categories fetched successfully.");
    }
}