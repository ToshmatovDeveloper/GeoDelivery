using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Dtos;
using Order.Infrastructure;

namespace Order.Application.Features;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;


public class GetOrderByIdQueryHandler(OrderDbContext dbContext) 
    : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        
        if (order == null)
        {
            return null;
        }

        return new OrderDto(
            order.Id,
            order.RestaurantId,
            order.CustomerId,
            order.Status.ToString(),
            order.DeliveryAddress.Latitude,
            order.DeliveryAddress.Longitude,
            order.TotalAmount,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemResponseDto(
                i.DishId,
                i.DishName,
                i.Quantity,
                i.Price,
                i.TotalPrice
            )).ToList()
        );
    }
}
