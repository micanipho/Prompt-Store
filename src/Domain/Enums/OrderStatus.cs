namespace Domain.Enums;

/// <summary>Represents the lifecycle stages of an order.</summary>
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
