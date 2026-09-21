using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using FluentValidation;
using MediatR;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Catalog.Application.Features.Restaurant.Command;

public record CreateRestaurantCommand(string Name, string Description) : IRequest<CreateRestaurantResponse>;

public record CreateRestaurantResponse(RestaurantDto? Dto, string Message);

public class CreateRestaurantCommandHandler(
    CatalogDbContext dbContext,
    IValidator<CreateRestaurantCommand> validator,
    ILogger<CreateRestaurantCommandHandler> logger) : IRequestHandler<CreateRestaurantCommand, CreateRestaurantResponse>
{
    public async Task<CreateRestaurantResponse> Handle(
        CreateRestaurantCommand command, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating command for restaurant {Name}...", command.Name);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for CreateRestaurantCommand. Errors: {@Errors}",
                validationResult.Errors);
            
            throw new ValidationException(validationResult.Errors.ToString());
        }

        logger.LogInformation("Creating restaurant {Name}...", command.Name);
        
        var restaurant = new Domain.Entity.Restaurant(command.Name, command.Description);

        await dbContext.Restaurants.AddAsync(restaurant, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new RestaurantDto(
            restaurant.Id, 
            restaurant.Name, 
            restaurant.Description, 
            restaurant.IsActive);

        logger.LogInformation("Restaurant {Name} created successfully with ID {RestaurantId}", 
            restaurant.Name, restaurant.Id);

        return new CreateRestaurantResponse(dto, "Restaurant created successfully.");
    }
}