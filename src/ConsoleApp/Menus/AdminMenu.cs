namespace ConsoleApp.Menus;

/// <summary>Displays the administrator menu with product, order, and reporting options.</summary>
public class AdminMenu(ProductService productService)
{
    private readonly ProductService _productService = productService;

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
            Console.WriteLine("4. View Products");
            Console.WriteLine("5. Logout");
            Console.Write("Please select an option: ");

            switch (Console.ReadLine())
            {
                case "1": AddProduct(); break;
                case "2": UpdateProduct(); break;
                case "3": DeleteProduct(); break;
                case "4": ViewProducts(); break;
                case "5":
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

        Console.Write("Name: ");
        var name = Console.ReadLine() ?? string.Empty;

        Console.Write("Description: ");
        var description = Console.ReadLine() ?? string.Empty;

        Console.Write("Category: ");
        var category = Console.ReadLine() ?? string.Empty;

        Console.Write("Price: ");
        if (!decimal.TryParse(Console.ReadLine(), out var price))
        {
            Console.WriteLine("Invalid price. Press any key to continue.");
            Console.ReadKey();
            return;
        }

        Console.Write("Stock: ");
        if (!int.TryParse(Console.ReadLine(), out var stock))
        {
            Console.WriteLine("Invalid stock quantity. Press any key to continue.");
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
        if (!int.TryParse(Console.ReadLine(), out var id))
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
            var priceInput = Console.ReadLine();
            var price = string.IsNullOrWhiteSpace(priceInput) ? existing.Price : decimal.Parse(priceInput);

            Console.Write($"Stock [{existing.Stock}]: ");
            var stockInput = Console.ReadLine();
            var stock = string.IsNullOrWhiteSpace(stockInput) ? existing.Stock : int.Parse(stockInput);

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
        if (!int.TryParse(Console.ReadLine(), out var id))
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

}
