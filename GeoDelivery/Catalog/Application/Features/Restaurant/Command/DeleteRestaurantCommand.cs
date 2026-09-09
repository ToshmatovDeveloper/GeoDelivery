using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Restaurant.Command;

public record DeleteRestaurantCommand(Guid RestaurantId) : IRequest<DeleteRestaurantResponse>;

public record DeleteRestaurantResponse(bool Result, string Message);

public class DeleteRestaurantCommandHandler(
    CatalogDbContext dbContext,
    ILogger<DeleteRestaurantCommandHandler> logger) : IRequestHandler<DeleteRestaurantCommand, DeleteRestaurantResponse>
{
    public async Task<DeleteRestaurantResponse> Handle(DeleteRestaurantCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Deleting restaurant {command.RestaurantId}...");

        var restaurant = await dbContext.Restaurants.FindAsync(command.RestaurantId , cancellationToken);

        if (restaurant is null)
        {
            return new DeleteRestaurantResponse(false, "Restaurant not found.");
        }

        dbContext.Restaurants.Remove(restaurant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteRestaurantResponse(true, "Restaurant deleted successfully.");
    }
}