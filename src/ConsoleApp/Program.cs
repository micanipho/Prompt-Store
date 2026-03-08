namespace ConsoleApp;

public class Program
{
    protected Program() { }

    internal static User? CurrentUser { get; set; }

    public static void Main(string[] args)
    {
        var userRepository = new InMemoryUserRepository();
        var productRepository = new InMemoryProductRepository();
        var orderRepository = new InMemoryOrderRepository();
        var paymentRepository = new InMemoryPaymentRepository();
        var authService = new AuthService(userRepository);
        var productService = new ProductService(productRepository);
        var cartService = new CartService(productRepository);
        var orderService = new OrderService(orderRepository, productRepository, paymentRepository);
        var paymentService = new PaymentService(paymentRepository);
        var inventoryService = new InventoryService(productRepository);
        var mainMenu = new MainMenu(authService, productService, cartService, orderService, paymentService, inventoryService);

        mainMenu.Show();
    }
}
