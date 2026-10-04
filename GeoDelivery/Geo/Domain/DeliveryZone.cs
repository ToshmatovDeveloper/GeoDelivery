using NetTopologySuite.Geometries;

namespace Geo.Domain;

public class DeliveryZone
{
    public DeliveryZone() { }

    public DeliveryZone(Guid restaurantId, string name, Polygon boundary, decimal deliveryCost)
    {
        Id = Guid.NewGuid();
        RestaurantId = restaurantId;
        Name = name;
        Boundary = boundary;
        Boundary.SRID = 4326; 
        DeliveryCost = deliveryCost;
        IsActive = true;
    }
    
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Polygon Boundary { get; set; } = null!;
    public decimal DeliveryCost { get; set; }
    public bool IsActive { get; set; }
}