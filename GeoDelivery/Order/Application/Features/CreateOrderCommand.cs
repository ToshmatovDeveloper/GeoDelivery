using MediatR;
using Order.Application.Dtos;
using Order.Domain.ValueObject;
using Order.Infrastructure;
using Shared.Contracts;

namespace Order.Application.Features;

public record CreateOrderCommand(
    Guid RestaurantId,
    Guid UserId,
    double Latitude,
    double Longitude,
    List<OrderItemDto> Items) : IRequest<CreateOrderResponse>;

public record CreateOrderResponse(Guid OrderId, string Message);    

public class CreateOrderCommandHandler(OrderDbContext dbContext, IMediator mediator)
    : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var deliveryAddress = new DeliveryAddress(request.Latitude, request.Longitude);
        
        var orderItems = request.Items
            .Select(i => new Domain.Entity.OrderItem(
                i.DishId, 
                i.DishName,
                i.Price,
                i.Quantity)).ToList();
        
        var order = new Domain.Entity.Order(
            request.UserId,
            request.RestaurantId,
            deliveryAddress,
            orderItems);
        
        dbContext.Orders.Add(order);
        
        await dbContext.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new OrderCreatedIntegrationEvent(
            order.Id,
            order.RestaurantId,
            request.Latitude,
            request.Longitude,
            order.CreatedAt
        ), cancellationToken);
        
        return new CreateOrderResponse(order.Id, "Order created successfully");
    }
}