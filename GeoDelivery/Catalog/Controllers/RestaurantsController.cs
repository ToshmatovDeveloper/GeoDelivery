using Catalog.Application.Features.Restaurant.Command;
using Catalog.Application.Features.Restaurant.Queries;
using Catalog.Domain.DTO_s;
using Catalog.Domain.DTO_s.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRestaurantDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateRestaurantCommand(dto.Name, dto.Description);
        
        var response = await mediator.Send(command, cancellationToken);

        if (response.Dto == null)
        {
            return BadRequest(new {response.Message});
        }
        
        return Ok(response.Dto);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRestaurantByIdQuery(id);
        
        var response = await mediator.Send(query, cancellationToken);

        if (response.Dto is null)
        {
            return NotFound(new { response.Message });
        }

        return Ok(response);
    }
    
    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        var query = new GetRestaurantByNameQuery(name);
        var response = await mediator.Send(query, cancellationToken);

        if (response.Dto is null)
        {
            return NotFound(new { response.Message });
        }

        return Ok(response);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteRestaurantCommand(id);
        var response = await mediator.Send(command, cancellationToken);

        if (!response.Result)
        {
            return NotFound(new { response.Message });
        }

        return Ok(response);
    }
}