using Catalog.Application.CustomExceptions;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Features.Category.Command;

public record CreateCategoryCommand(Guid RestaurantId, string Name) : IRequest<CreateCategoryResponse>;

public record CreateCategoryResponse(CategoryDto? Dto, string Message);

public class CreateCategoryCommandHandler(
    CatalogDbContext context,
    IValidator<CreateCategoryCommand> validator,
    ILogger<CreateCategoryCommandHandler> logger)
    : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating create category command for restaurant {RestaurantId} and name {Name}...", request.RestaurantId, request.Name);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for CreateCategoryCommand with {ErrorCount} errors.", validationResult.Errors.Count);
            throw new ValidationException(validationResult.Errors);
        }

        logger.LogInformation("Creating category for restaurant {RestaurantId} with name {Name}", request.RestaurantId, request.Name);
        
        var restaurantCheck = await context.Restaurants.FindAsync([request.RestaurantId], cancellationToken);
        
        if (restaurantCheck is null)
        {
            logger.LogWarning("Restaurant with ID {RestaurantId} not found.", request.RestaurantId);

            throw new NotFoundException($"Restaurant with ID {request.RestaurantId} not found.");
        }
        
        var category = new Domain.Entity.Category(request.RestaurantId, request.Name);

        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var categoryDto = new CategoryDto(category.Id, category.RestaurantId, category.Name);

        logger.LogInformation("Category {Name} created successfully with ID {CategoryId}.", category.Name, category.Id);

        return new CreateCategoryResponse(categoryDto, "Category created successfully.");
    }
}