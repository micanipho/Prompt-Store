namespace Domain.Entities;

/// <summary>Represents a customer's shopping cart containing items to be purchased.</summary>
public class Cart
{
    public List<CartItem> Items { get; private set; } = [];
}
