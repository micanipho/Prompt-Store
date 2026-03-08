namespace Application.Dtos;

/// <summary>Represents a summary of orders grouped by status.</summary>
public class OrderStatusSummary
{
    public OrderStatus Status { get; set; }
    public int Count { get; set; }
    public decimal Total { get; set; }
}
