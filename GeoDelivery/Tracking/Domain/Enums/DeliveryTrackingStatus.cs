namespace Tracking.Domain.Enums;

public enum DeliveryTrackingStatus
{
    PendingAssignment = 0,    
    EnRouteToRestaurant = 1,  
    AtRestaurant = 2,
    Delivering = 3,  
    Delivered = 4,   
    Cancelled = 5    
}