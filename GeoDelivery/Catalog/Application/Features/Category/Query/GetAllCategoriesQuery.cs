using Catalog.Application.Caching;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Category.Query;

public record GetAllCategoriesQuery() : IRequest<GetListOfAllCategoriesResponse>, ICachableQuery
{
    public string CacheKey => "categories:all";
}

public record GetListOfAllCategoriesResponse(IEnumerable<CategoryDto> Categories, string Message);

public class GetListOfAllCategoriesQueryHandler(
    CatalogDbContext context, 
    ILogger<GetListOfAllCategoriesQueryHandler> logger) 
    : IRequestHandler<GetAllCategoriesQuery, GetListOfAllCategoriesResponse>
{
    public async Task<GetListOfAllCategoriesResponse> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all categories");
        
        var categories = await context.Categories
            .Select(c => new CategoryDto(c.Id, c.RestaurantId, c.Name))
            .ToListAsync(cancellationToken);

        if (categories.Count == 0)
        {
            logger.LogInformation("No categories found in the database.");
        }

        return new GetListOfAllCategoriesResponse(categories, "Categories fetched successfully.");
    }
}