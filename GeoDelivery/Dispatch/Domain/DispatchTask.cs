namespace Dispatch.Domain;

public class DispatchTask
{
    private DispatchTask() { }

    public DispatchTask(Guid orderId, Guid restaurantId, double latitude, double longitude)
    {
        OrderId = orderId;
        RestaurantId = restaurantId;
        Latitude = latitude;
        Longitude = longitude;
        IsProcessed = false;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public bool IsProcessed { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}