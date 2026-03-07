namespace ConsoleApp.Menus;

public class RegisterMenu
{
    private readonly AuthService _authService;

    public RegisterMenu(AuthService authService)
    {
        _authService = authService;
    }

    public void Show()
    {
        Console.Clear();
        Console.WriteLine("=== Register ===");
        Console.Write("Enter username: ");
        var username = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter password: ");
        var password = ConsoleHelper.ReadPassword();

        var request = new RegisterUserRequest
        {
            UserName = username,
            Password = password,
            Role = UserRole.Customer
        };

        try
        {
            _authService.Register(request);
            Console.WriteLine("Registration successful! Press any key to return to the main menu.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration failed: {ex.Message}");
            Console.WriteLine("Press any key to return to the main menu.");
        }

        Console.ReadKey();
    }

}