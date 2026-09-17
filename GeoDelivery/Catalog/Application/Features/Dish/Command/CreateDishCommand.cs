using Catalog.Application.CustomExceptions;
using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        logger.LogInformation("Creating dish {Name} for restaurant {RestaurantId}...", command.Name, command.RestaurantId);

        // Проверяем существование ресторана
        var restaurantExists = await dbContext.Restaurants
            .AnyAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (!restaurantExists)
        {
            logger.LogWarning("Restaurant with ID {RestaurantId} not found.", command.RestaurantId);
            throw new NotFoundException($"Restaurant with ID {command.RestaurantId} was not found.");
        }

        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == command.CategoryId && c.RestaurantId == command.RestaurantId, cancellationToken);

        if (category is null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found for restaurant {RestaurantId}.", command.CategoryId, command.RestaurantId);
            throw new NotFoundException($"Category with ID {command.CategoryId} for restaurant {command.RestaurantId} was not found.");
        }

        var dish = new Domain.Entity.Dish(command.RestaurantId, command.CategoryId, command.Name, command.Price);

        await dbContext.Dishes.AddAsync(dish, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new DishDto(dish.Id, dish.RestaurantId, dish.CategoryId, dish.Name, dish.Price, dish.IsAvailable);

        logger.LogInformation("Dish {Name} created successfully with ID {DishId}.", dish.Name, dish.Id);
        
        return new CreateDishResponse(dto, "Dish created successfully.");
    }
}