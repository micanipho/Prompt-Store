namespace ConsoleApp;

public class Program
{
    protected Program() { }

    internal static User? CurrentUser { get; set; }

    public static void Main(string[] args)
    {
        var userRepository = new InMemoryUserRepository();
        var productRepository = new InMemoryProductRepository();
        var authService = new AuthService(userRepository);
        var productService = new ProductService(productRepository);
        var cartService = new CartService(productRepository);
        var mainMenu = new MainMenu(authService, productService, cartService);

        mainMenu.Show();
    }
}
