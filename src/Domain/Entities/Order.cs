namespace Domain.Entities;

/// <summary>Represents a placed order containing one or more products.</summary>
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.Now;
}
