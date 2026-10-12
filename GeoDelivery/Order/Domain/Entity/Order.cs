using Order.Domain.Enums;
using Order.Domain.ValueObject;

namespace Order.Domain.Entity;

public class Order
{
    private Order() { }

    public Order(
        Guid customerId, 
        Guid restaurantId, 
        DeliveryAddress deliveryAddress, 
        IEnumerable<OrderItem> items)
    {
        CustomerId = customerId;
        RestaurantId = restaurantId;
        DeliveryAddress = deliveryAddress;
        
        var orderItems = items?.ToList() ?? [];
        _items.AddRange(orderItems);
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public Guid? CourierId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public DeliveryAddress DeliveryAddress { get; private set; } = default!;
    public decimal TotalAmount { get; private set; }
    
    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}