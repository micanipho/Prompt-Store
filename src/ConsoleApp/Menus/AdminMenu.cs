namespace ConsoleApp.Menus;

/// <summary>Displays the administrator menu with product, order, inventory, and reporting options.</summary>
public class AdminMenu(ProductService productService, OrderService orderService, InventoryService inventoryService, ReportService reportService)
{
    private readonly ProductService _productService = productService;
    private readonly OrderService _orderService = orderService;
    private readonly InventoryService _inventoryService = inventoryService;
    private readonly ReportService _reportService = reportService;

    /// <summary>Displays the administrator menu in a loop until the user logs out.</summary>
    public void Show()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader($"Administrator Menu \u2502 {Program.CurrentUser?.UserName}");

            ConsoleHelper.PrintSubHeader("Product Management");
            ConsoleHelper.PrintMenuOption("1", "Add Product");
            ConsoleHelper.PrintMenuOption("2", "Update Product");
            ConsoleHelper.PrintMenuOption("3", "Delete Product");
            ConsoleHelper.PrintMenuOption("4", "Restock Product");
            ConsoleHelper.PrintMenuOption("5", "View Products");
            ConsoleHelper.PrintMenuOption("6", "View Low Stock Products");
            Console.WriteLine();

            ConsoleHelper.PrintSubHeader("Order Management");
            ConsoleHelper.PrintMenuOption("7", "View Orders");
            ConsoleHelper.PrintMenuOption("8", "Update Order Status");
            Console.WriteLine();

            ConsoleHelper.PrintSubHeader("Analytics");
            ConsoleHelper.PrintMenuOption("9", "Generate Sales Reports");
            Console.WriteLine();

            ConsoleHelper.PrintSeparator();
            ConsoleHelper.PrintMenuOption("10", "Logout");
            Console.WriteLine();
            ConsoleHelper.PrintPrompt("Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": AddProduct(); break;
                case "2": UpdateProduct(); break;
                case "3": DeleteProduct(); break;
                case "4": RestockProduct(); break;
                case "5": ViewProducts(); break;
                case "6": ViewLowStockProducts(); break;
                case "7": ViewOrders(); break;
                case "8": UpdateOrderStatus(); break;
                case "9": GenerateSalesReport(); break;
                case "10":
                    Program.CurrentUser = null;
                    ConsoleHelper.PrintSuccess("Logged out successfully.");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    return;
                default:
                    ConsoleHelper.PrintError("Invalid option. Please try again.");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    break;
            }
        }
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

            Console.Write($"  Price [{existing.Price:F2}]: ");
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

}
