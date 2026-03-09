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
    private readonly PaymentService _paymentService;
    private readonly IPdfGenerator _pdfGenerator;

    public LoginMenu(AuthService authService, ProductService productService, CartService cartService, OrderService orderService, InventoryService inventoryService, ReportService reportService, ReviewService reviewService, PaymentService paymentService, IPdfGenerator pdfGenerator)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _inventoryService = inventoryService;
        _reportService = reportService;
        _reviewService = reviewService;
        _paymentService = paymentService;
        _pdfGenerator = pdfGenerator;
    }

    /// <summary>Prompts for credentials, authenticates the user, and opens the role-specific menu.</summary>
    public void Show()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Login");

        Console.Write("  Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("  Password: ");
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
            ConsoleHelper.PrintSuccess($"Welcome back, {user.UserName}!");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);

            if (user.Role == UserRole.Admin)
            {
                new AdminMenu(_productService, _orderService, _inventoryService, _reportService, _pdfGenerator).Show();
            }
            else
            {
                new CustomerMenu((Customer)user, _productService, _cartService, _orderService, _reviewService, _paymentService).Show();
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Login failed: {ex.Message}");
            ConsoleHelper.PressAnyKey("Press any key to return to the main menu...");
        }
    }
}
