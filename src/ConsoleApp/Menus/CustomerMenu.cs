namespace ConsoleApp.Menus;

/// <summary>Displays the customer menu with shopping, order, and wallet options.</summary>
public class CustomerMenu(AuthService authService, ProductService productService)
{
    private readonly AuthService _authService = authService;
    private readonly ProductService _productService = productService;

    /// <summary>Displays the customer menu in a loop until the user logs out.</summary>
    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== Customer Menu === [{Program.CurrentUser?.UserName}]");
            Console.WriteLine("1. Browse Products");
            Console.WriteLine("2. Search Products");
            Console.WriteLine("3. Logout");
            Console.Write("Please select an option: ");

            switch (Console.ReadLine())
            {
                case "1": BrowseProducts(); break;
                case "2": SearchProducts(); break;
                case "3":
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
}
