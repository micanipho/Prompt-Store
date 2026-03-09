namespace Domain.Entities;

/// <summary>Represents a customer's shopping cart containing items to be purchased.</summary>
public class Cart
{
    public virtual int Id { get; protected set; }
    public virtual IList<CartItem> Items { get; protected set; } = new List<CartItem>();
}
