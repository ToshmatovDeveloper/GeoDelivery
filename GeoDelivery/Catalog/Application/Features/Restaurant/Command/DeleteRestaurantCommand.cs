using Catalog.Application.CustomExceptions;
using Catalog.Infrastructure;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Features.Restaurant.Command;

public record DeleteRestaurantCommand(Guid RestaurantId) : IRequest<DeleteRestaurantResponse>;

public record DeleteRestaurantResponse(bool Result, string Message);

public class DeleteRestaurantCommandHandler(
    CatalogDbContext dbContext,
    IValidator<DeleteRestaurantCommand> validator,
    ILogger<DeleteRestaurantCommandHandler> logger) : IRequestHandler<DeleteRestaurantCommand, DeleteRestaurantResponse>
{
    public async Task<DeleteRestaurantResponse> Handle(
        DeleteRestaurantCommand command, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating delete command for restaurant {RestaurantId}...", 
            command.RestaurantId);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for DeleteRestaurantCommand with {ErrorCount} errors",
                validationResult.Errors.Count);
            throw new ValidationException(validationResult.Errors);
        }

        logger.LogInformation("Deleting restaurant with ID {RestaurantId}...", command.RestaurantId);

        var restaurant = await dbContext.Restaurants
            .FindAsync([command.RestaurantId], cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with ID {RestaurantId} not found for deletion",
                command.RestaurantId);
            throw new NotFoundException($"Restaurant {command.RestaurantId} not found.");
        }

        dbContext.Restaurants.Remove(restaurant);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Restaurant with ID {RestaurantId} deleted successfully", 
            command.RestaurantId);

        return new DeleteRestaurantResponse(true, "Restaurant deleted successfully.");
    }
}