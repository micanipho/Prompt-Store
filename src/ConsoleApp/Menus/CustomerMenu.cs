namespace ConsoleApp.Menus;

/// <summary>Displays the customer menu with shopping, order, and wallet options.</summary>
public class CustomerMenu(AuthService authService)
{
    private readonly AuthService _authService = authService;

    public void Show()
    {
        // TODO: implement customer menu options
        Console.WriteLine("Customer menu coming soon.");
    }
}
