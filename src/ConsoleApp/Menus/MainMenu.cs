namespace ConsoleApp.Menus;

/// <summary>Displays the application entry menu with options to register, login, or exit.</summary>
public class MainMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;

    public MainMenu(AuthService authService, ProductService productService)
    {
        _authService = authService;
        _productService = productService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Welcome to the Prompt Store ===");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
            Console.Write("Please select an option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var loginMenu = new LoginMenu(_authService, _productService);
                    loginMenu.Show();
                    break;
                case "2":
                    var registerMenu = new RegisterMenu(_authService);
                    registerMenu.Show();
                    break;
                case "3":
                    Console.WriteLine("Thank you for visiting the Prompt Store. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Thread.Sleep(2000);
                    break;
            }
        }
    }
}