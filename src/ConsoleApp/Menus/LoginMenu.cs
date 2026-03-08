namespace ConsoleApp.Menus;

public class LoginMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly PaymentService _paymentService;
    private readonly InventoryService _inventoryService;
    private readonly ReportService _reportService;

    public LoginMenu(AuthService authService, ProductService productService, CartService cartService, OrderService orderService, PaymentService paymentService, InventoryService inventoryService, ReportService reportService)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _reportService = reportService;
    }

    public void Show()
    {
        Console.Clear();
        Console.WriteLine("=== Login ===");
        Console.Write("Username: ");
        var username = Console.ReadLine() ?? string.Empty;

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
            Thread.Sleep(2000);

            if (user.Role == UserRole.Admin)
            {
                new AdminMenu(_productService, _orderService, _inventoryService, _reportService).Show();
            }
            else
            {
                new CustomerMenu((Customer)user, _productService, _cartService, _orderService, _paymentService).Show();
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