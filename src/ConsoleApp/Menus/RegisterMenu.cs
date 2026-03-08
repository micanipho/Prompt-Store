namespace ConsoleApp.Menus;

/// <summary>Handles new user registration, collecting credentials and role selection.</summary>
public class RegisterMenu
{
    private readonly AuthService _authService;

    public RegisterMenu(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Prompts for username, password, and role, then registers the new account.</summary>
    public void Show()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Register");

        var username = ConsoleHelper.ReadNonEmptyString("  Enter username: ");

        Console.Write("  Enter password: ");
        var password = ConsoleHelper.ReadPassword();

        if (string.IsNullOrWhiteSpace(password))
        {
            ConsoleHelper.PrintError("Password cannot be empty.");
            ConsoleHelper.PressAnyKey("Press any key to return to the main menu...");
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintSubHeader("Account Type");
        ConsoleHelper.PrintMenuOption("1", "Customer");
        ConsoleHelper.PrintMenuOption("2", "Administrator");
        Console.WriteLine();
        ConsoleHelper.PrintPrompt("Choice: ");
        var roleChoice = Console.ReadLine()?.Trim();

        UserRole role;
        if (roleChoice == "2")
            role = UserRole.Admin;
        else if (roleChoice == "1")
            role = UserRole.Customer;
        else
        {
            ConsoleHelper.PrintError("Invalid choice.");
            ConsoleHelper.PressAnyKey("Press any key to return to the main menu...");
            return;
        }

        var request = new RegisterUserRequest
        {
            UserName = username,
            Password = password,
            Role = role
        };

        try
        {
            _authService.Register(request);
            ConsoleHelper.PrintSuccess("Registration successful!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Registration failed: {ex.Message}");
        }

        ConsoleHelper.PressAnyKey("Press any key to return to the main menu...");
    }

}
