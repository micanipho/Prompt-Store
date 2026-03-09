namespace Domain.Entities;

/// <summary>Represents a single product line within an order.</summary>
public class OrderItem
{
    public virtual int Id { get; protected set; }
    public virtual Product Product { get; set; } = null!;
    public virtual int Quantity { get; set; }
    public virtual decimal UnitPrice { get; set; }
    public virtual Order Order { get; set; } = null!;
}
