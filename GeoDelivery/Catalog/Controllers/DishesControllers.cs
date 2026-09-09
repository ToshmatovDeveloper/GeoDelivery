using Catalog.Application.Features.Dish.Command;
using Catalog.Application.Features.Dish.Queries;
using Catalog.Domain.DTO_s.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;

[ApiController]
[Route("api/restaurants/dishes")]
public class DishesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDishDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateDishCommand(dto.RestaurantId, dto.CategoryId, dto.Name, dto.Price);
        
        var response = await mediator.Send(command, cancellationToken);

        return Ok(response);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid restaurantId, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDishById(id);
        
        var response = await mediator.Send(query, cancellationToken);

        if (response is null || response.Dto.RestaurantId != restaurantId)
        {
            return NotFound(new { response.Message });
        }

        return Ok(response);
    }

    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        var query = new GetDishByNameQuery(name);
        
        var response = await mediator.Send(query, cancellationToken);

        if (response.Dto is null)
        {
            return NotFound(new { response.Message });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetByRestaurant(Guid restaurantId, CancellationToken cancellationToken)
    {
        var query = new GetDishListByRestaurantIdQuery(restaurantId);
        
        var response = await mediator.Send(query, cancellationToken);

        if (response.Dishes is null)
        {
            return NotFound(new { response.Message });
        }

        return Ok(response);
    }

    [HttpPatch("{id:guid}/unavailable")]
    public async Task<IActionResult> MakeUnavailable(Guid id, CancellationToken cancellationToken)
    {
        var command = new MakeDishUnavailableCommand(id);
        
        var response = await mediator.Send(command, cancellationToken);

        if (!response.IsUnavailable)
        {
            return NotFound(new { response.Message });
        }

        return Ok(response);
    }
}