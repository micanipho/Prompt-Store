namespace Domain.Entities;

/// <summary>Represents a placed order containing one or more products.</summary>
public class Order
{
    public virtual int Id { get; set; }
    public virtual IList<OrderItem> Items { get; set; } = new List<OrderItem>();
    public virtual decimal Total { get; set; }
    public virtual OrderStatus Status { get; set; }
    public virtual DateTime PlacedAt { get; set; } = DateTime.Now;
    public virtual Customer? Customer { get; set; }
}
