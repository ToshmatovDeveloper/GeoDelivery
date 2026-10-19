using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Dtos;
using Order.Application.Features;

namespace Order.Presenters;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder(
        [FromBody] CreateOrderCommand command, 
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = response.OrderId }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var order = await mediator.Send(query, cancellationToken);

        if (order == null)
        {
            return NotFound(new { Message = $"Order with ID '{id}' not found." });
        }

        return Ok(order);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(
        Guid id, 
        [FromBody] Guid userId, 
        CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand(id, userId);
        var success = await mediator.Send(command, cancellationToken);

        if (!success)
        {
            return BadRequest(new { Message = "Unable to cancel order. Either order does not exist or user is unauthorized." });
        }

        return NoContent(); 
    }
}