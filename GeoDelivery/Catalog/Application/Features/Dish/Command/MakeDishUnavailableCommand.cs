using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Dish.Command;

public record MakeDishUnavailableCommand(Guid DishId) : IRequest<MakeDishUnavailableResponse>;
public record MakeDishUnavailableResponse(bool IsUnavailable, string Message);

public class MakeDishUnavailableCommandHandler(
    CatalogDbContext dbContext,
    ILogger<MakeDishUnavailableCommandHandler> logger) : IRequestHandler<MakeDishUnavailableCommand, MakeDishUnavailableResponse>
{
    public async Task<MakeDishUnavailableResponse> Handle(MakeDishUnavailableCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Making dish {command.DishId} unavailable...");

        var dish = await dbContext.Dishes.FindAsync(command.DishId, cancellationToken);

        if (dish is null)
        {
            return new MakeDishUnavailableResponse(false, "Dish not found.");
        }

        dish.IsAvailable = false;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Dish {Name} made unavailable successfully.", dish.Name);

        return new MakeDishUnavailableResponse(true, "Dish made unavailable successfully.");
    }
}