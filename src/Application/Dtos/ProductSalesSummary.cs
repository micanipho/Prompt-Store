namespace Application.Dtos;

/// <summary>Represents sales data for a single product.</summary>
public class ProductSalesSummary
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
