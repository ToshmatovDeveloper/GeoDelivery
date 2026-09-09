using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Category.Command;

public record CreateCategoryCommand(Guid RestaurantId, string Name) : IRequest<CreateCategoryResponse>;

public record CreateCategoryResponse(CategoryDto? Dto, string Message);

public class CreateCategoryCommandHandler(
    CatalogDbContext context,
    ILogger<CreateCategoryCommandHandler> logger)
    : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Creating category {request.RestaurantId} with name {request.Name}");
        
        var restaurantCheck = await context.Restaurants.FindAsync(request.RestaurantId);
        
        if (restaurantCheck is null)
        {
            logger.LogWarning($"Restaurant with ID {request.RestaurantId} not found.");
            
            return new CreateCategoryResponse(null, $"Restaurant with ID {request.RestaurantId} not found.");
        }
        
        var category = new Domain.Entity.Category(request.RestaurantId, request.Name);

        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var categoryDto = new CategoryDto(category.Id, category.RestaurantId, category.Name);

        return new CreateCategoryResponse(categoryDto, "Category created successfully.");
    }
}