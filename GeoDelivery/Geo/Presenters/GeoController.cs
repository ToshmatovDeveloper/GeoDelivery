using Geo.Application.Features;
using Geo.Application.Features.PostGisGeo;
using Geo.Application.Features.RedisGeo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geo.Presenters;

[ApiController]
[Route("api/geo")]
[Authorize] // Требует авторизации по умолчанию для всех методов, если не указано иное
public class GeoController(IMediator mediator) : ControllerBase
{
    [HttpPost("zones")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateDeliveryZone(
        [FromBody] CreateDeliveryZoneCommand command, 
        CancellationToken token)
    {
        var response = await mediator.Send(command, token);
        return Ok(response);
    }

    [HttpGet("zones/{zoneId:guid}/check")]
    [Authorize(Roles = "Admin,Manager,Client")]
    public async Task<IActionResult> CheckPointInZone(
        Guid zoneId,
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken token)
    {
        var query = new CheckPointInZoneQuery(zoneId, latitude, longitude);
        var response = await mediator.Send(query, token);
        return Ok(response);
    }

    [HttpGet("zones/nearest")]
    [Authorize(Roles = "Admin,Manager,Client")]
    public async Task<IActionResult> GetNearestDeliveryZone(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken token)
    {
        var query = new GetNearestDeliveryZoneQuery(latitude, longitude);
        var response = await mediator.Send(query, token);

        if (response is null)
        {
            return NotFound(new { Message = "No active delivery zones found." });
        }

        return Ok(response);
    }

    [HttpGet("distance")]
    [Authorize(Roles = "Admin,Manager,Client")]
    public async Task<IActionResult> CalculateDistance(
        [FromQuery] double lat1,
        [FromQuery] double lon1,
        [FromQuery] double lat2,
        [FromQuery] double lon2,
        CancellationToken token)
    {
        var query = new CalculateDistanceQuery(lat1, lon1, lat2, lon2);
        var response = await mediator.Send(query, token);
        return Ok(response);
    }

    [HttpPost("couriers/location")]
    [Authorize(Roles = "Courier")]
    public async Task<IActionResult> UpdateCourierLocation(
        [FromBody] UpdateCourierLocationCommand command,
        CancellationToken token)
    {
        await mediator.Send(command, token);
        return NoContent();
    }

    [HttpGet("couriers/nearby")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetNearbyCouriers(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusInKm,
        CancellationToken token)
    {
        var query = new GetNearbyCouriersQuery(latitude, longitude, radiusInKm);
        var response = await mediator.Send(query, token);
        return Ok(response);
    }

    [HttpDelete("couriers/{courierId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveCourier(
        Guid courierId,
        CancellationToken token)
    {
        var command = new RemoveCourierCommand(courierId);
        await mediator.Send(command, token);
        return NoContent();
    }
}