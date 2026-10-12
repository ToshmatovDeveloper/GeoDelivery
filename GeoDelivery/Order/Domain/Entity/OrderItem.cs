namespace Order.Domain.Entity;

public class OrderItem
{
    private OrderItem() { }

    public OrderItem(Guid dishId, string dishName, decimal price, int quantity)
    {
        DishId = dishId;
        DishName = dishName;
        Price = price;
        Quantity = quantity;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid DishId { get; private set; }
    public string DishName { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => Price * Quantity;
}