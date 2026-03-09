namespace Domain.Entities;

/// <summary>Represents a single product entry within a shopping cart.</summary>
public class CartItem
{
    public virtual int Id { get; protected set; }
    public virtual Product Product { get; set; } = null!;
    public virtual int Quantity { get; set; }
    public virtual Cart Cart { get; set; } = null!;
}
