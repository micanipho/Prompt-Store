namespace ConsoleApp;

/// <summary>Application entry point that wires up repositories, services, and launches the main menu.</summary>
public class Program
{
    protected Program() { }

    /// <summary>The currently logged-in user, or null when no user is authenticated.</summary>
    internal static User? CurrentUser { get; set; }

    /// <summary>Initialises all dependencies and starts the console application.</summary>
    public static void Main(string[] args)
    {
        var userRepository = new InMemoryUserRepository();
        var productRepository = new InMemoryProductRepository();
        var orderRepository = new InMemoryOrderRepository();
        var paymentRepository = new InMemoryPaymentRepository();
        var reviewRepository = new InMemoryReviewRepository();
        var authService = new AuthService(userRepository);
        var productService = new ProductService(productRepository);
        var cartService = new CartService(productRepository);
        var orderService = new OrderService(orderRepository, productRepository, paymentRepository);
        var paymentService = new PaymentService(paymentRepository);
        var inventoryService = new InventoryService(productRepository);
        var reportService = new ReportService(orderRepository, paymentRepository);
        var reviewService = new ReviewService(reviewRepository, productRepository);
        var mainMenu = new MainMenu(authService, productService, cartService, orderService, paymentService, inventoryService, reportService, reviewService);

        mainMenu.Show();
    }
}
