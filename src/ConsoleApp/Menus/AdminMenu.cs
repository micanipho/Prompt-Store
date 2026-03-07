namespace ConsoleApp.Menus;

/// <summary>Displays the administrator menu with product, order, and reporting options.</summary>
public class AdminMenu(AuthService authService)
{
    private readonly AuthService _authService = authService;

    public void Show()
    {
        // TODO: implement admin menu options
        Console.WriteLine("Admin menu coming soon.");
    }
}
