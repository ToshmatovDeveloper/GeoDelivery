using Catalog.Application.Caching;
using Catalog.Application.CustomExceptions;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Dish.Queries;

public record GetDishByNameQuery(string Name) : IRequest<GetDishByNameResponse>, ICachableQuery
{
    public string CacheKey => $"dish:name:{Name.ToLowerInvariant()}";
}

public record GetDishByNameResponse(DishDto? Dto, string Message);

public class GetDishByNameQueryHandler(
    CatalogDbContext dbContext,
    ILogger<GetDishByNameQueryHandler> logger) : IRequestHandler<GetDishByNameQuery, GetDishByNameResponse>
{
    public async Task<GetDishByNameResponse> Handle(GetDishByNameQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching dish {Name}...", request.Name);

        var dish = await dbContext.Dishes
            .FirstOrDefaultAsync(d => d.Name == request.Name, cancellationToken);

        if (dish is null)
        {
            logger.LogInformation("Dish {Name} not found.", request.Name);
            throw new NotFoundException($"Dish {request.Name} not found.");
        }

        var dto = new DishDto(dish.Id, dish.RestaurantId, dish.CategoryId, dish.Name, dish.Price, dish.IsAvailable);

        logger.LogInformation("Dish {Name} fetched successfully.", request.Name);
        
        return new GetDishByNameResponse(dto, "Dish fetched successfully.");
    }
}