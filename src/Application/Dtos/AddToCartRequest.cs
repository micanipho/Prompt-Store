namespace Application.Dtos;

/// <summary>Request to add a product to the customer's shopping cart.</summary>
public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
