using Catalog.Application.Features.Category.Command;
using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Category.Query;

public record GetAllCategoriesQuery() : IRequest<GetListOfAllCategoriesResponse>;
public record GetListOfAllCategoriesResponse(IEnumerable<CategoryDto> Categories, string Message);

public class GetListOfAllCategoriesQueryHandler(
    CatalogDbContext context, 
    ILogger<DeleteCategoryCommandHandler> logger) : IRequestHandler<GetAllCategoriesQuery, GetListOfAllCategoriesResponse>
{
    public async Task<GetListOfAllCategoriesResponse> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all categories");
        
        var categories = await context.Categories
            .Select(c => new CategoryDto(c.Id, c.RestaurantId, c.Name))
            .ToListAsync(cancellationToken);

        return new GetListOfAllCategoriesResponse(categories, "Categories fetched successfully.");
    }
}