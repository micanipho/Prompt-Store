namespace Domain.Entities;

/// <summary>Represents a single product line within an order.</summary>
public class OrderItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
