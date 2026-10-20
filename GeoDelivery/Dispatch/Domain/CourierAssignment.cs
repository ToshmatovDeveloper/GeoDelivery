namespace Dispatch.Domain;

public class CourierAssignment
{
    private CourierAssignment() { }

    public CourierAssignment(Guid orderId, Guid restaurantId, Guid courierId)
    {
        OrderId = orderId;
        RestaurantId = restaurantId;
        CourierId = courierId;
        Status = AssignmentStatus.Assigned;
        AssignedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public Guid CourierId { get; private set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;
    public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}