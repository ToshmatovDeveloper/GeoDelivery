using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Restaurant.Command;

public record CreateRestaurantCommand(string Name, string Description) : IRequest<CreateRestaurantResponse>;

public record CreateRestaurantResponse(RestaurantDto? Dto, string Message);

public class CreateRestaurantCommandHandler(
    CatalogDbContext dbContext,
    ILogger<CreateRestaurantCommandHandler> logger) : IRequestHandler<CreateRestaurantCommand, CreateRestaurantResponse>
{
    public async Task<CreateRestaurantResponse> Handle(CreateRestaurantCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Creating restaurant {command.Name}...");
        
        var restaurant = new Domain.Entity.Restaurant(command.Name, command.Description);

        await dbContext.Restaurants.AddAsync(restaurant, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new RestaurantDto(restaurant.Id, restaurant.Name, restaurant.Description, restaurant.IsActive);

        return new CreateRestaurantResponse(dto, "Restaurant created successfully.");
    }
}

