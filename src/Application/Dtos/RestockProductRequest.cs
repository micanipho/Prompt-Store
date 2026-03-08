namespace Application.Dtos;

/// <summary>Carries the input data required to restock an existing product.</summary>
public class RestockProductRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
