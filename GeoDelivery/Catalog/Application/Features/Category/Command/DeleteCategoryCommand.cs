using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Category.Command;

public record DeleteCategoryCommand(Guid CategoryId, Guid RestaurantId) : IRequest<DeleteCategoryResponse>;
public record DeleteCategoryResponse(bool Success, string? Message) ;

public class DeleteCategoryCommandHandler(
    CatalogDbContext context,
    ILogger<DeleteCategoryCommandHandler> logger)
    : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResponse>
{
    public async Task<DeleteCategoryResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Deleting category {request.CategoryId} for restaurant {request.RestaurantId}");
        
        var category = await context.Categories.FindAsync(request.CategoryId);
        
        if (category is null || category.RestaurantId != request.RestaurantId)
        {
            logger.LogWarning($"Category with ID {request.CategoryId} not found for restaurant {request.RestaurantId}.");
            return new DeleteCategoryResponse(false, $"Category with ID {request.CategoryId} not found for restaurant {request.RestaurantId}.");
        }
        
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        
        return new DeleteCategoryResponse(true, "Category deleted successfully.");
    }
}