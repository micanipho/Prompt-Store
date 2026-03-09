using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");

        var options = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var context = new ShoppingDbContext(options);

        DatabaseSeeder.Seed(context);

        IUnitOfWork unitOfWork = new EfUnitOfWork(context);
        var userRepository = new EfUserRepository(context);
        var productRepository = new EfProductRepository(context);
        var orderRepository = new EfOrderRepository(context);
        var paymentRepository = new EfPaymentRepository(context);
        var reviewRepository = new EfReviewRepository(context);
        var authService = new AuthService(userRepository);
        var productService = new ProductService(productRepository);
        var cartService = new CartService(productRepository, unitOfWork);
        var orderService = new OrderService(orderRepository, productRepository, paymentRepository, unitOfWork);
        var paymentService = new PaymentService(paymentRepository, unitOfWork);
        var inventoryService = new InventoryService(productRepository);
        var reportService = new ReportService(orderRepository, paymentRepository);
        var reviewService = new ReviewService(reviewRepository, productRepository);
        var mainMenu = new MainMenu(authService, productService, cartService, orderService, inventoryService, reportService, reviewService, paymentService);

        try
        {
            mainMenu.Show();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nAn unexpected error occurred. The application will now exit.");
            Console.WriteLine($"Details: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
