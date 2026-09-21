using Catalog.Application.CustomExceptions;
using Catalog.Infrastructure;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Features.Dish.Command;

public record MakeDishUnavailableCommand(Guid DishId) : IRequest<MakeDishUnavailableResponse>;
public record MakeDishUnavailableResponse(bool IsUnavailable, string Message);

public class MakeDishUnavailableCommandHandler(
    CatalogDbContext dbContext,
    IValidator<MakeDishUnavailableCommand> validator,
    ILogger<MakeDishUnavailableCommandHandler> logger) : IRequestHandler<MakeDishUnavailableCommand, MakeDishUnavailableResponse>
{
    public async Task<MakeDishUnavailableResponse> Handle(MakeDishUnavailableCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating make dish unavailable command for dish {DishId}...", command.DishId);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for MakeDishUnavailableCommand with {ErrorCount} errors.", validationResult.Errors.Count);
            throw new ValidationException(validationResult.Errors);
        }

        logger.LogInformation("Making dish {DishId} unavailable...", command.DishId);

        var dish = await dbContext.Dishes.FindAsync([command.DishId], cancellationToken);

        if (dish is null)
        {
            logger.LogWarning("Dish with ID {DishId} not found.", command.DishId);
            throw new NotFoundException($"Dish with ID {command.DishId} not found.");
        }

        dish.IsAvailable = false;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Dish {Name} made unavailable successfully", dish.Name);

        return new MakeDishUnavailableResponse(true, "Dish made unavailable successfully.");
    }
}