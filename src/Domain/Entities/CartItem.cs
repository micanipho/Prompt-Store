namespace Domain.Entities;

/// <summary>Represents a single product entry within a shopping cart.</summary>
public class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
}
