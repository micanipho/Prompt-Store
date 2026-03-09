using Infrastructure.Services;
using QuestPDF.Infrastructure;

namespace Infrastructure.Tests;

public class QuestPdfGeneratorTests
{
    private readonly QuestPdfGenerator _generator;

    public QuestPdfGeneratorTests()
    {
        _generator = new QuestPdfGenerator();
    }

    [Fact]
    public void GenerateSalesReport_ShouldCreateFile()
    {
        // Arrange
        var filePath = "test_sales_report.pdf";
        if (File.Exists(filePath)) File.Delete(filePath);

        var statusSummaries = new List<OrderStatusSummary>
        {
            new() { Status = OrderStatus.Pending, Count = 5, Total = 500 },
            new() { Status = OrderStatus.Shipped, Count = 2, Total = 300 }
        };

        var topProducts = new List<ProductSalesSummary>
        {
            new() { ProductId = 1, ProductName = "Product A", TotalQuantitySold = 10, TotalRevenue = 1000 }
        };

        var dailySales = new List<DailySalesSummary>
        {
            new() { Date = DateTime.Today, OrderCount = 3, Revenue = 800 }
        };

        // Act
        _generator.GenerateSalesReport(10, 2000, 200, statusSummaries, topProducts, dailySales, filePath);

        // Assert
        Assert.True(File.Exists(filePath));

        // Clean up
        if (File.Exists(filePath)) File.Delete(filePath);
    }

    [Fact]
    public void GenerateInventoryReport_ShouldCreateFile()
    {
        // Arrange
        var filePath = "test_inventory_report.pdf";
        if (File.Exists(filePath)) File.Delete(filePath);

        var products = new List<Product>
        {
            new() { Id = 1, Name = "Product A", Category = "Cat A", Price = 100, Stock = 10 },
            new() { Id = 2, Name = "Product B", Category = "Cat B", Price = 50, Stock = 3 },
            new() { Id = 3, Name = "Product C", Category = "Cat C", Price = 30, Stock = 0 }
        };

        var lowStock = products.Where(p => p.Stock <= 5 && p.Stock > 0).ToList();
        var outOfStock = products.Where(p => p.Stock == 0).ToList();

        // Act
        _generator.GenerateInventoryReport(products, lowStock, outOfStock, filePath);

        // Assert
        Assert.True(File.Exists(filePath));

        // Clean up
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}
