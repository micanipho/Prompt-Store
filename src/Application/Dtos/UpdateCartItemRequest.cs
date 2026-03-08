namespace Application.Dtos;

/// <summary>Request to update the quantity of a product in the cart. Set NewQuantity to 0 to remove the item.</summary>
public class UpdateCartItemRequest
{
    public int ProductId { get; set; }
    public int NewQuantity { get; set; }
}
