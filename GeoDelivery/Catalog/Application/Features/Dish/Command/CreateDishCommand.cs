using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Dish.Command;

public record CreateDishCommand(Guid RestaurantId, Guid CategoryId, string Name, decimal Price) : IRequest<CreateDishResponse>;

public record CreateDishResponse(DishDto Dto, string Message);

public class CreateDishCommandHandler(
    CatalogDbContext dbContext,
    ILogger<CreateDishCommandHandler> logger) : IRequestHandler<CreateDishCommand, CreateDishResponse>
{
    public async Task<CreateDishResponse> Handle(CreateDishCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Creating dish {command.Name} for restaurant {command.RestaurantId}...");

        var dish = new Domain.Entity.Dish(command.RestaurantId, command.CategoryId, command.Name, command.Price);

        await dbContext.Dishes.AddAsync(dish, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new DishDto(dish.Id, dish.RestaurantId, dish.CategoryId, dish.Name, dish.Price, dish.IsAvailable);

        logger.LogInformation("Dish {Name} created successfully.", dish.Name);
        
        return new CreateDishResponse(dto, "Dish created successfully.");
    }
}