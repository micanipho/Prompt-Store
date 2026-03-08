namespace ConsoleApp.Menus;

/// <summary>Handles the login flow, authenticating the user and routing to the appropriate role menu.</summary>
public class LoginMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly InventoryService _inventoryService;
    private readonly ReportService _reportService;
    private readonly ReviewService _reviewService;

    public LoginMenu(AuthService authService, ProductService productService, CartService cartService, OrderService orderService, InventoryService inventoryService, ReportService reportService, ReviewService reviewService)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _inventoryService = inventoryService;
        _reportService = reportService;
        _reviewService = reviewService;
    }

    /// <summary>Prompts for credentials, authenticates the user, and opens the role-specific menu.</summary>
    public void Show()
    {
        Console.Clear();
        Console.WriteLine("=== Login ===");
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Password: ");
        var password = ConsoleHelper.ReadPassword();

        var loginRequest = new LoginRequest
        {
            UserName = username,
            Password = password
        };

        try
        {
            var user = _authService.Login(loginRequest);
            Program.CurrentUser = user;
            Console.WriteLine($"Welcome back, {user.UserName}!");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);

            if (user.Role == UserRole.Admin)
            {
                new AdminMenu(_productService, _orderService, _inventoryService, _reportService).Show();
            }
            else
            {
                new CustomerMenu((Customer)user, _productService, _cartService, _orderService, _reviewService).Show();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }
    }
}