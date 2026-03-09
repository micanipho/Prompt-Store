using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Factories;
using Domain.Interfaces;
using Domain.Strategies;

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
        var configuration = BuildConfiguration();
        var serviceProvider = ConfigureServices(configuration);

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ShoppingDbContext>();
            DatabaseSeeder.Seed(context);

            var mainMenu = scope.ServiceProvider.GetRequiredService<MainMenu>();

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

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();
    }

    private static IServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");

        services.AddDbContext<ShoppingDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Infrastructure
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IPaymentRepository, EfPaymentRepository>();
        services.AddScoped<IReviewRepository, EfReviewRepository>();

        // Application Services
        services.AddScoped<IPdfGenerator, Infrastructure.Services.QuestPdfGenerator>();
        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IOrderFactory, OrderFactory>();
        services.AddScoped<IProductFactory, ProductFactory>();
        services.AddScoped<IDiscountStrategy>(sp => new PercentageDiscountStrategy("Submission 2 Special (10% Off)", 0.10m));
        services.AddScoped<AuthService>();
        services.AddScoped<ProductService>();
        services.AddScoped<CartService>();
        services.AddScoped<OrderService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<InventoryService>();
        services.AddScoped<ReportService>();
        services.AddScoped<ReviewService>();

        // Presentation
        services.AddScoped<MainMenu>();

        return services.BuildServiceProvider();
    }
}
