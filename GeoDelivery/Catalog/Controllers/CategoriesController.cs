using Catalog.Application.Features.Category.Command;
using Catalog.Application.Features.Category.Query;
using Catalog.Domain.DTO_s.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;

[ApiController]
[Route("api/restaurants/categories")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryDto dto,
        CancellationToken token)
    {
        var command = new CreateCategoryCommand(dto.RestaurantId, dto.Name);
        
        var response = await mediator.Send(command, token);

        if (response.Dto is  null) 
        {
            return BadRequest(new {response.Message});
        }
        
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetByRestaurant(Guid restaurantId, CancellationToken token)
    {
        var query = new GetCategoriesByRestaurantQuery(restaurantId);
        
        var response = await mediator.Send(query, token);

        if (response.Categories is null)
        {
            return NotFound(new {response.Message});
        }
        
        return Ok(response);
    }

    [HttpGet("/api/categories")]
    public async Task<IActionResult> GetAllCategories(CancellationToken token)
    {
        var query = new GetAllCategoriesQuery();
        
        var response = await mediator.Send(query, token);
        
        return Ok(response);
    }

    [HttpGet("dishes/{dishId:guid}")]
    public async Task<IActionResult> GetCategoryOfDish(
        Guid restaurantId,
        Guid dishId,
        CancellationToken token)
    {
        var query = new GetCategoryOfDishQuery(dishId, restaurantId);
        
        var response = await mediator.Send(query, token);
        
        return Ok(response);
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> Delete(
        Guid restaurantId,
        Guid categoryId,
        CancellationToken token)
    {
        var command = new DeleteCategoryCommand(categoryId,  restaurantId);

        var response = await mediator.Send(command, token);

        if (!response.Success)
        {
            return BadRequest(response.Message);
        }
        
        return Ok(response);
    }
}