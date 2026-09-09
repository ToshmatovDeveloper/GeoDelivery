using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Get;
using Catalog.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Features.Dish.Queries;

public record GetDishById(Guid Id) : IRequest<GetDishByIdResponse>;

public record GetDishByIdResponse(DishDto Dto, string Message);

public class GetDishByIdHandler(
    CatalogDbContext dbContext,
    ILogger<GetDishByIdHandler> logger) : IRequestHandler<GetDishById, GetDishByIdResponse>
{
    public async Task<GetDishByIdResponse> Handle(GetDishById query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching dish {Id}...", query.Id);

        var dish = await dbContext.Dishes.FindAsync(new object[] { query.Id }, cancellationToken);

        if (dish is null)
        {
            logger.LogInformation("Dish {Id} not found.", query.Id);
            return new GetDishByIdResponse(null, "Dish not found.");
        }

        var dto = new DishDto(dish.Id, dish.RestaurantId, dish.CategoryId, dish.Name, dish.Price, dish.IsAvailable);

        logger.LogInformation("Dish {Id} fetched successfully.", query.Id);
        return new GetDishByIdResponse(dto, "Dish fetched successfully.");
    }
}

