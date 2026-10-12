namespace Order.Domain.Enums;

public enum OrderStatus
{
    Created = 1,
    Accepted = 2,
    Cooking = 3,
    ReadyForDelivery = 4,
    Delivering = 5,
    Completed = 6,
    Cancelled = 7
}