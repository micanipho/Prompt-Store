namespace ConsoleApp.Menus;

/// <summary>Displays the administrator menu with product, order, inventory, and reporting options.</summary>
public class AdminMenu : BaseMenu
{
    private readonly ProductService _productService;
    private readonly OrderService _orderService;
    private readonly InventoryService _inventoryService;
    private readonly ReportService _reportService;
    private readonly IPdfGenerator _pdfGenerator;

    public AdminMenu(ProductService productService, OrderService orderService, InventoryService inventoryService, ReportService reportService, IPdfGenerator pdfGenerator)
    {
        _productService = productService;
        _orderService = orderService;
        _inventoryService = inventoryService;
        _reportService = reportService;
        _pdfGenerator = pdfGenerator;

        // Register Commands
        AddCommand("1", "Add Product", AddProduct);
        AddCommand("2", "Update Product", UpdateProduct);
        AddCommand("3", "Delete Product", DeleteProduct);
        AddCommand("4", "Restock Product", RestockProduct);
        AddCommand("5", "View Products", ViewProducts);
        AddCommand("6", "View Low Stock Products", ViewLowStockProducts);
        AddCommand("7", "View Orders", ViewOrders);
        AddCommand("8", "Update Order Status", UpdateOrderStatus);
        AddCommand("9", "Generate Sales Reports", GenerateSalesReport);
        AddCommand("10", "Export Sales Report (PDF)", ExportSalesReportPdf);
        AddCommand("11", "Export Inventory (PDF)", ExportInventoryPdf);
    }

    protected override string Header => "Administrator Menu";

    protected override void PrintMenuContent()
    {
        ConsoleHelper.PrintSubHeader("Product Management");
        ConsoleHelper.PrintMenuOption("1", _commands["1"].Description);
        ConsoleHelper.PrintMenuOption("2", _commands["2"].Description);
        ConsoleHelper.PrintMenuOption("3", _commands["3"].Description);
        ConsoleHelper.PrintMenuOption("4", _commands["4"].Description);
        ConsoleHelper.PrintMenuOption("5", _commands["5"].Description);
        ConsoleHelper.PrintMenuOption("6", _commands["6"].Description);
        ConsoleHelper.PrintMenuOption("11", _commands["11"].Description);
        Console.WriteLine();

        ConsoleHelper.PrintSubHeader("Order Management");
        ConsoleHelper.PrintMenuOption("7", _commands["7"].Description);
        ConsoleHelper.PrintMenuOption("8", _commands["8"].Description);
        Console.WriteLine();

        ConsoleHelper.PrintSubHeader("Analytics");
        ConsoleHelper.PrintMenuOption("9", _commands["9"].Description);
        ConsoleHelper.PrintMenuOption("10", _commands["10"].Description);
        Console.WriteLine();
    }

    public override void Show()
    {
        base.Show();
        // Logout logic when base.Show returns (input "0")
        Program.CurrentUser = null;
        ConsoleHelper.PrintSuccess("Logged out successfully.");
        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }

    private void AddProduct()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Add Product");

        var name = ConsoleHelper.ReadNonEmptyString("  Name: ");
        var description = ConsoleHelper.ReadNonEmptyString("  Description: ");
        var category = ConsoleHelper.ReadNonEmptyString("  Category: ");
        var price = ConsoleHelper.ReadPositiveDecimal("  Price: ");

        Console.Write("  Stock: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var stock) || stock < 0)
        {
            ConsoleHelper.PrintError("Invalid stock quantity. Must be zero or a positive number.");
            ConsoleHelper.PressAnyKey();
            return;
        }

        try
        {
            _productService.AddProduct(new CreateProductRequest
            {
                Name = name,
                Description = description,
                Category = category,
                Price = price,
                Stock = stock
            });
            ConsoleHelper.PrintSuccess("Product added successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        ConsoleHelper.PressAnyKey();
    }

    private void UpdateProduct()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Update Product");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());

        Console.WriteLine();
        if (!ConsoleHelper.TryReadInt("  Enter Product ID to update: ", out var id))
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        try
        {
            var existing = _productService.GetProductById(id);

            Console.Write($"  Name [{existing.Name}]: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = existing.Name;

            Console.Write($"  Description [{existing.Description}]: ");
            var description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description)) description = existing.Description;

            Console.Write($"  Category [{existing.Category}]: ");
            var category = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(category)) category = existing.Category;

            Console.Write($"  Price [R{existing.Price:F2}]: ");
            var priceInput = Console.ReadLine()?.Trim();
            decimal price;
            if (string.IsNullOrWhiteSpace(priceInput))
                price = existing.Price;
            else if (!decimal.TryParse(priceInput, out price) || price <= 0)
            {
                ConsoleHelper.PrintError("Invalid price. Must be a positive number.");
                ConsoleHelper.PressAnyKey();
                return;
            }

            Console.Write($"  Stock [{existing.Stock}]: ");
            var stockInput = Console.ReadLine()?.Trim();
            int stock;
            if (string.IsNullOrWhiteSpace(stockInput))
                stock = existing.Stock;
            else if (!int.TryParse(stockInput, out stock) || stock < 0)
            {
                ConsoleHelper.PrintError("Invalid stock. Must be zero or a positive number.");
                ConsoleHelper.PressAnyKey();
                return;
            }

            _productService.UpdateProduct(new UpdateProductRequest
            {
                Id = id,
                Name = name,
                Description = description,
                Category = category,
                Price = price,
                Stock = stock
            });

            ConsoleHelper.PrintSuccess("Product updated successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        ConsoleHelper.PressAnyKey();
    }

    private void DeleteProduct()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Delete Product");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());

        Console.WriteLine();
        if (!ConsoleHelper.TryReadInt("  Enter Product ID to delete: ", out var id))
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        try
        {
            _productService.DeleteProduct(id);
            ConsoleHelper.PrintSuccess("Product deleted successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        ConsoleHelper.PressAnyKey();
    }

    private void ViewProducts()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("View Products");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());
        ConsoleHelper.PressAnyKey();
    }

    private void ViewOrders()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("All Orders");
        ConsoleHelper.PrintOrderTable(_orderService.GetAllOrders());
        ConsoleHelper.PressAnyKey();
    }

    private void UpdateOrderStatus()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Update Order Status");
        ConsoleHelper.PrintOrderTable(_orderService.GetAllOrders());

        Console.WriteLine();
        if (!ConsoleHelper.TryReadInt("  Enter Order ID: ", out var orderId))
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintSubHeader("Available Statuses");
        foreach (var status in Enum.GetValues<OrderStatus>())
            ConsoleHelper.PrintMenuOption($"{(int)status}", $"{status}");

        Console.WriteLine();
        ConsoleHelper.PrintPrompt("Select new status: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var statusValue) || !Enum.IsDefined(typeof(OrderStatus), statusValue))
        {
            ConsoleHelper.PrintError("Invalid status.");
            ConsoleHelper.PressAnyKey();
            return;
        }

        try
        {
            _orderService.UpdateOrderStatus(new UpdateOrderStatusRequest
            {
                OrderId = orderId,
                NewStatus = (OrderStatus)statusValue
            });
            ConsoleHelper.PrintSuccess("Order status updated successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        ConsoleHelper.PressAnyKey();
    }

    private void RestockProduct()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Restock Product");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());

        Console.WriteLine();
        if (!ConsoleHelper.TryReadInt("  Enter Product ID to restock: ", out var productId))
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        if (!ConsoleHelper.TryReadInt("  Enter quantity to add: ", out var quantity))
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        try
        {
            _inventoryService.RestockProduct(new RestockProductRequest
            {
                ProductId = productId,
                Quantity = quantity
            });
            var newStock = _inventoryService.GetStockLevel(productId);
            ConsoleHelper.PrintSuccess($"Product restocked successfully. New stock level: {newStock}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        ConsoleHelper.PressAnyKey();
    }

    private void ViewLowStockProducts()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Low Stock Products (Stock \u2264 5)");
        ConsoleHelper.PrintProductTable(_inventoryService.GetLowStockProducts());
        ConsoleHelper.PressAnyKey();
    }

    private void GenerateSalesReport()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Sales Report");

        SalesReportPrinter.PrintSalesReport(
            _reportService.GetTotalOrders(),
            _reportService.GetTotalRevenue(),
            _reportService.GetAverageOrderValue(),
            _reportService.GetOrdersByStatus(),
            _reportService.GetTopSellingProducts(),
            _reportService.GetDailySales());

        ConsoleHelper.PressAnyKey();
    }

    private void ExportSalesReportPdf()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Export Sales Report to PDF");

        try
        {
            var reportsDir = Path.Combine(ConsoleHelper.GetProjectRoot(), "Reports");
            if (!Directory.Exists(reportsDir)) Directory.CreateDirectory(reportsDir);

            var fileName = $"Sales_Report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf";
            var filePath = Path.Combine(reportsDir, fileName);

            Console.WriteLine("  Generating PDF...");
            _pdfGenerator.GenerateSalesReport(
                _reportService.GetTotalOrders(),
                _reportService.GetTotalRevenue(),
                _reportService.GetAverageOrderValue(),
                _reportService.GetOrdersByStatus(),
                _reportService.GetTopSellingProducts(),
                _reportService.GetDailySales(),
                filePath);

            ConsoleHelper.PrintSuccess($"Report exported successfully to:");
            Console.WriteLine($"  {filePath}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Failed to export report: {ex.Message}");
        }

        ConsoleHelper.PressAnyKey();
    }

    private void ExportInventoryPdf()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Export Inventory to PDF");

        try
        {
            var reportsDir = Path.Combine(ConsoleHelper.GetProjectRoot(), "Reports");
            if (!Directory.Exists(reportsDir)) Directory.CreateDirectory(reportsDir);

            var fileName = $"Inventory_Report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf";
            var filePath = Path.Combine(reportsDir, fileName);

            Console.WriteLine("  Generating PDF...");
            _pdfGenerator.GenerateInventoryReport(
                _productService.GetAllProducts(),
                _inventoryService.GetLowStockProducts(),
                _inventoryService.GetOutOfStockProducts(),
                filePath);

            ConsoleHelper.PrintSuccess($"Inventory exported successfully to:");
            Console.WriteLine($"  {filePath}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Failed to export inventory: {ex.Message}");
        }

        ConsoleHelper.PressAnyKey();
    }

}
