namespace Application.Interfaces;

/// <summary>Defines a contract for generating PDF reports for sales and inventory.</summary>
public interface IPdfGenerator
{
    /// <summary>Generates a PDF sales report and saves it to the specified path.</summary>
    void GenerateSalesReport(
        int totalOrders,
        decimal totalRevenue,
        decimal averageOrderValue,
        IEnumerable<OrderStatusSummary> statusSummaries,
        IEnumerable<ProductSalesSummary> topProducts,
        IEnumerable<DailySalesSummary> dailySales,
        string filePath);

    /// <summary>Generates a PDF inventory report and saves it to the specified path.</summary>
    void GenerateInventoryReport(
        IEnumerable<Product> products,
        IEnumerable<Product> lowStockProducts,
        IEnumerable<Product> outOfStockProducts,
        string filePath);
}
