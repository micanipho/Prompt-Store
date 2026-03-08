namespace ConsoleApp.Menus;

/// <summary>Displays the application entry menu with options to register, login, or exit.</summary>
public class MainMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly InventoryService _inventoryService;
    private readonly ReportService _reportService;
    private readonly ReviewService _reviewService;

    public MainMenu(AuthService authService, ProductService productService, CartService cartService, OrderService orderService, InventoryService inventoryService, ReportService reportService, ReviewService reviewService)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _inventoryService = inventoryService;
        _reportService = reportService;
        _reviewService = reviewService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintBanner();
            ConsoleHelper.PrintHeader("Main Menu");

            ConsoleHelper.PrintMenuOption("1", "Login");
            ConsoleHelper.PrintMenuOption("2", "Register");
            ConsoleHelper.PrintMenuOption("3", "Exit");
            Console.WriteLine();
            ConsoleHelper.PrintPrompt("Select an option: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    var loginMenu = new LoginMenu(_authService, _productService, _cartService, _orderService, _inventoryService, _reportService, _reviewService);
                    loginMenu.Show();
                    break;
                case "2":
                    var registerMenu = new RegisterMenu(_authService);
                    registerMenu.Show();
                    break;
                case "3":
                    Console.WriteLine();
                    ConsoleHelper.PrintSuccess("Thank you for visiting the Prompt Store. Goodbye!");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    return;
                default:
                    ConsoleHelper.PrintError("Invalid option. Please try again.");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    break;
            }
        }
    }
}
