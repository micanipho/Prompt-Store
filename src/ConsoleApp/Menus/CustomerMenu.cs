namespace ConsoleApp.Menus;

/// <summary>Displays the customer menu with shopping, order, and wallet options.</summary>
public class CustomerMenu
{
    private readonly Customer _customer;
    private readonly ProductService _productService;
    private readonly CartService _cartService;

    public CustomerMenu(Customer customer, ProductService productService, CartService cartService)
    {
        _customer = customer;
        _productService = productService;
        _cartService = cartService;
    }

    /// <summary>Displays the customer menu in a loop until the user logs out.</summary>
    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== Customer Menu === [{Program.CurrentUser?.UserName}]");
            Console.WriteLine("1. Browse Products");
            Console.WriteLine("2. Search Products");
            Console.WriteLine("3. Add Product to Cart");
            Console.WriteLine("4. View Cart");
            Console.WriteLine("5. Update Cart");
            Console.WriteLine("6. Logout");
            Console.Write("Please select an option: ");

            switch (Console.ReadLine())
            {
                case "1": BrowseProducts(); break;
                case "2": SearchProducts(); break;
                case "3": AddProductToCart(); break;
                case "4": ViewCart(); break;
                case "5": UpdateCart(); break;
                case "6":
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

    private void BrowseProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Browse Products ===\n");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());
        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }

    private void SearchProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Search Products ===");
        Console.WriteLine("1. Search by Name");
        Console.WriteLine("2. Search by Category");
        Console.Write("Select search type: ");

        var choice = Console.ReadLine();
        IEnumerable<Product> results;

        if (choice == "1")
        {
            Console.Write("Enter product name: ");
            results = _productService.SearchByName(Console.ReadLine() ?? string.Empty);
        }
        else if (choice == "2")
        {
            Console.Write("Enter category: ");
            results = _productService.SearchByCategory(Console.ReadLine() ?? string.Empty);
        }
        else
        {
            Console.WriteLine("Invalid option. Press any key to continue.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintProductTable(results);
        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }

    private void AddProductToCart()
    {
        Console.Clear();
        Console.WriteLine("=== Add Product to Cart ===\n");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());

        Console.Write("\nEnter Product ID: ");
        if (!int.TryParse(Console.ReadLine(), out var productId))
        {
            Console.WriteLine("Invalid Product ID.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        Console.Write("Enter Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity))
        {
            Console.WriteLine("Invalid quantity.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            _cartService.AddToCart(_customer, new AddToCartRequest
            {
                ProductId = productId,
                Quantity = quantity
            });
            Console.WriteLine("Product added to cart successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }

    private void ViewCart()
    {
        Console.Clear();
        Console.WriteLine("=== Your Cart ===\n");
        var items = _cartService.GetCartItems(_customer);
        var total = _cartService.GetCartTotal(_customer);
        ConsoleHelper.PrintCartTable(items, total);
        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }

    private void UpdateCart()
    {
        Console.Clear();
        Console.WriteLine("=== Update Cart ===\n");
        var items = _cartService.GetCartItems(_customer);
        var total = _cartService.GetCartTotal(_customer);
        ConsoleHelper.PrintCartTable(items, total);

        if (!items.Any())
        {
            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey();
            return;
        }

        Console.Write("\nEnter Product ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var productId))
        {
            Console.WriteLine("Invalid Product ID.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        Console.Write("Enter new quantity (0 to remove): ");
        if (!int.TryParse(Console.ReadLine(), out var newQuantity))
        {
            Console.WriteLine("Invalid quantity.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            _cartService.UpdateCartItem(_customer, new UpdateCartItemRequest
            {
                ProductId = productId,
                NewQuantity = newQuantity
            });
            Console.WriteLine(newQuantity == 0
                ? "Product removed from cart."
                : "Cart updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }
}
