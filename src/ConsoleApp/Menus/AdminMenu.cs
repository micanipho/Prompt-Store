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
            Console.WriteLine($"=== Administrator Menu === [{Program.CurrentUser?.UserName}]");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Update Product");
            Console.WriteLine("3. Delete Product");
            Console.WriteLine("4. Restock Product");
            Console.WriteLine("5. View Products");
            Console.WriteLine("6. View Low Stock Products");
            Console.WriteLine("7. View Orders");
            Console.WriteLine("8. Update Order Status");
            Console.WriteLine("9. Generate Sales Reports");
            Console.WriteLine("10. Logout");
            Console.Write("Please select an option: ");

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
                    Console.WriteLine("Logged out successfully.");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    break;
            }
        }
    }

    private void AddProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Add Product ===");

        var name = ConsoleHelper.ReadNonEmptyString("Name: ");
        var description = ConsoleHelper.ReadNonEmptyString("Description: ");
        var category = ConsoleHelper.ReadNonEmptyString("Category: ");
        var price = ConsoleHelper.ReadPositiveDecimal("Price: ");

        Console.Write("Stock: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var stock) || stock < 0)
        {
            Console.WriteLine("Invalid stock quantity. Must be zero or a positive number. Press any key to continue.");
            Console.ReadKey();
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
            Console.WriteLine("Product added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }

    private void UpdateProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Update Product ===\n");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());

        Console.Write("\nEnter Product ID to update: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue.");
            Console.ReadKey();
            return;
        }

        try
        {
            var existing = _productService.GetProductById(id);

            Console.Write($"Name [{existing.Name}]: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = existing.Name;

            Console.Write($"Description [{existing.Description}]: ");
            var description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description)) description = existing.Description;

            Console.Write($"Category [{existing.Category}]: ");
            var category = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(category)) category = existing.Category;

            Console.Write($"Price [{existing.Price:F2}]: ");
            var priceInput = Console.ReadLine()?.Trim();
            decimal price;
            if (string.IsNullOrWhiteSpace(priceInput))
                price = existing.Price;
            else if (!decimal.TryParse(priceInput, out price) || price <= 0)
            {
                Console.WriteLine("Invalid price. Must be a positive number.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                return;
            }

            Console.Write($"Stock [{existing.Stock}]: ");
            var stockInput = Console.ReadLine()?.Trim();
            int stock;
            if (string.IsNullOrWhiteSpace(stockInput))
                stock = existing.Stock;
            else if (!int.TryParse(stockInput, out stock) || stock < 0)
            {
                Console.WriteLine("Invalid stock. Must be zero or a positive number.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
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

            Console.WriteLine("Product updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }

    private void DeleteProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Product ===\n");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());

        Console.Write("\nEnter Product ID to delete: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue.");
            Console.ReadKey();
            return;
        }

        try
        {
            _productService.DeleteProduct(id);
            Console.WriteLine("Product deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }

    private void ViewProducts()
    {
        Console.Clear();
        Console.WriteLine("=== View Products ===\n");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());
        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }

    private void ViewOrders()
    {
        Console.Clear();
        Console.WriteLine("=== All Orders ===\n");
        ConsoleHelper.PrintOrderTable(_orderService.GetAllOrders());
        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }

    private void UpdateOrderStatus()
    {
        Console.Clear();
        Console.WriteLine("=== Update Order Status ===\n");
        ConsoleHelper.PrintOrderTable(_orderService.GetAllOrders());

        Console.Write("\nEnter Order ID: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var orderId))
        {
            Console.WriteLine("Invalid Order ID. Press any key to continue.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nStatuses:");
        foreach (var status in Enum.GetValues<OrderStatus>())
            Console.WriteLine($"  {(int)status}. {status}");

        Console.Write("Select new status: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var statusValue) || !Enum.IsDefined(typeof(OrderStatus), statusValue))
        {
            Console.WriteLine("Invalid status. Press any key to continue.");
            Console.ReadKey();
            return;
        }

        try
        {
            _orderService.UpdateOrderStatus(new UpdateOrderStatusRequest
            {
                OrderId = orderId,
                NewStatus = (OrderStatus)statusValue
            });
            Console.WriteLine("Order status updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }

    private void RestockProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Restock Product ===\n");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());

        Console.Write("\nEnter Product ID to restock: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var productId))
        {
            Console.WriteLine("Invalid ID. Press any key to continue.");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter quantity to add: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var quantity))
        {
            Console.WriteLine("Invalid quantity. Press any key to continue.");
            Console.ReadKey();
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
            Console.WriteLine($"Product restocked successfully. New stock level: {newStock}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }

    private void ViewLowStockProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Low Stock Products (Stock <= 5) ===\n");
        ConsoleHelper.PrintProductTable(_inventoryService.GetLowStockProducts());
        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }

    private void GenerateSalesReport()
    {
        Console.Clear();
        Console.WriteLine("=== Sales Report ===\n");

        ConsoleHelper.PrintSalesReport(
            _reportService.GetTotalOrders(),
            _reportService.GetTotalRevenue(),
            _reportService.GetAverageOrderValue(),
            _reportService.GetOrdersByStatus(),
            _reportService.GetTopSellingProducts(),
            _reportService.GetDailySales());

        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }

}
