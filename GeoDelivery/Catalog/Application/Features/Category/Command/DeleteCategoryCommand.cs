using Catalog.Application.CustomExceptions;
using Catalog.Infrastructure;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Features.Category.Command;

public record DeleteCategoryCommand(Guid CategoryId, Guid RestaurantId) : IRequest<DeleteCategoryResponse>;
public record DeleteCategoryResponse(bool Success, string? Message) ;

public class DeleteCategoryCommandHandler(
    CatalogDbContext context,
    IValidator<DeleteCategoryCommand> validator,
    ILogger<DeleteCategoryCommandHandler> logger)
    : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResponse>
{
    public async Task<DeleteCategoryResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating delete category command for category {CategoryId} and restaurant {RestaurantId}...", request.CategoryId, request.RestaurantId);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for DeleteCategoryCommand with {ErrorCount} errors.", validationResult.Errors.Count);
            throw new ValidationException(validationResult.Errors);
        }

        logger.LogInformation("Deleting category {CategoryId} for restaurant {RestaurantId}", request.CategoryId, request.RestaurantId);
        
        var category = await context.Categories.FindAsync([request.CategoryId], cancellationToken);
        
        if (category is null || category.RestaurantId != request.RestaurantId)
        {
            logger.LogWarning("Category with ID {CategoryId} not found for restaurant {RestaurantId}.", request.CategoryId, request.RestaurantId);
            throw new NotFoundException(
                $"Category with ID {request.CategoryId} not found for restaurant {request.RestaurantId}.");
        }
        
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Category {CategoryId} deleted successfully.", request.CategoryId);

        return new DeleteCategoryResponse(true, "Category deleted successfully.");
    }
}