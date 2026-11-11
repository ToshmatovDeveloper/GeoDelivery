namespace Tracking.Domain.Entities;

public class CourierLocationPoint
{
    private CourierLocationPoint() { } 
    
    public CourierLocationPoint(Guid courierId, Guid? orderId, double latitude, double longitude)
    {
        Id = Guid.NewGuid();
        CourierId = courierId;
        OrderId = orderId;
        Latitude = latitude;
        Longitude = longitude;
        Timestamp = DateTime.UtcNow;
    }
    
    public Guid Id { get; private set; }
    public Guid CourierId { get; private set; }
    public Guid? OrderId { get; private set; } 
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public DateTime Timestamp { get; private set; }
}