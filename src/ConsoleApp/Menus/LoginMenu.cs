namespace ConsoleApp.Menus;

public class LoginMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;

    public LoginMenu(AuthService authService, ProductService productService)
    {
        _authService = authService;
        _productService = productService;
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
                new AdminMenu(_authService, _productService).Show();
            }
            else
            {
                new CustomerMenu(_authService, _productService).Show();
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