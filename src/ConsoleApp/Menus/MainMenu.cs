namespace ConsoleApp.Menus;

/// <summary>Displays the application entry menu with options to register, login, or exit.</summary>
public class MainMenu : BaseMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly InventoryService _inventoryService;
    private readonly ReportService _reportService;
    private readonly ReviewService _reviewService;
    private readonly PaymentService _paymentService;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly ShoppingAssistantService _assistantService;

    public MainMenu(AuthService authService, ProductService productService, CartService cartService, OrderService orderService, InventoryService inventoryService, ReportService reportService, ReviewService reviewService, PaymentService paymentService, IPdfGenerator pdfGenerator, ShoppingAssistantService assistantService)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _inventoryService = inventoryService;
        _reportService = reportService;
        _reviewService = reviewService;
        _paymentService = paymentService;
        _pdfGenerator = pdfGenerator;
        _assistantService = assistantService;

        // Register Commands
        AddCommand("1", "Login", ShowLogin);
        AddCommand("2", "Register", ShowRegister);
    }

    protected override string Header => "Main Menu";

    protected override void PrintMenuContent()
    {
        ConsoleHelper.PrintBanner();
        ConsoleHelper.PrintMenuOption("1", _commands["1"].Description);
        ConsoleHelper.PrintMenuOption("2", _commands["2"].Description);
    }

    public override void Show()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader($"{Header}");

            PrintMenuContent();

            ConsoleHelper.PrintSeparator();
            ConsoleHelper.PrintMenuOption("0", "Exit");
            Console.WriteLine();
            ConsoleHelper.PrintPrompt("Select an option: ");

            var input = Console.ReadLine()?.Trim();
            if (input == "0")
            {
                Console.WriteLine();
                ConsoleHelper.PrintSuccess("Thank you for visiting the Prompt Store. Goodbye!");
                Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                return;
            }

            if (_commands.TryGetValue(input ?? string.Empty, out var command))
            {
                command.Execute();
            }
            else
            {
                ConsoleHelper.PrintError("Invalid option. Please try again.");
                Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            }
        }
    }

    private void ShowLogin()
    {
        new LoginMenu(_authService, _productService, _cartService, _orderService, _inventoryService, _reportService, _reviewService, _paymentService, _pdfGenerator, _assistantService).Show();
    }

    private void ShowRegister()
    {
        new RegisterMenu(_authService).Show();
    }
}
