namespace ConsoleApp.Menus;

/// <summary>Displays the customer menu with shopping, order, and wallet options.</summary>
public partial class CustomerMenu
{
    private readonly Customer _customer;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly ReviewService _reviewService;
    private readonly PaymentService _paymentService;

    public CustomerMenu(Customer customer, ProductService productService, CartService cartService, OrderService orderService, ReviewService reviewService, PaymentService paymentService)
    {
        _customer = customer;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _reviewService = reviewService;
        _paymentService = paymentService;
    }

    /// <summary>Displays the customer menu in a loop until the user logs out.</summary>
    public void Show()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader($"Customer Menu \u2502 {Program.CurrentUser?.UserName}");

            ConsoleHelper.PrintSubHeader("Shopping");
            ConsoleHelper.PrintMenuOption("1", "Browse Products");
            ConsoleHelper.PrintMenuOption("2", "Search Products");
            ConsoleHelper.PrintMenuOption("3", "Add Product to Cart");
            Console.WriteLine();

            ConsoleHelper.PrintSubHeader("Cart & Checkout");
            ConsoleHelper.PrintMenuOption("4", "View Cart");
            ConsoleHelper.PrintMenuOption("5", "Update Cart");
            ConsoleHelper.PrintMenuOption("6", "Checkout");
            Console.WriteLine();

            ConsoleHelper.PrintSubHeader("Wallet");
            ConsoleHelper.PrintMenuOption("7", "View Wallet Balance");
            ConsoleHelper.PrintMenuOption("8", "Add Wallet Funds");
            Console.WriteLine();

            ConsoleHelper.PrintSubHeader("Orders & Reviews");
            ConsoleHelper.PrintMenuOption("9", "View Order History");
            ConsoleHelper.PrintMenuOption("10", "Track Orders");
            ConsoleHelper.PrintMenuOption("11", "Review Products");
            Console.WriteLine();

            ConsoleHelper.PrintSeparator();
            ConsoleHelper.PrintMenuOption("12", "Logout");
            Console.WriteLine();
            ConsoleHelper.PrintPrompt("Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": BrowseProducts(); break;
                case "2": SearchProducts(); break;
                case "3": AddProductToCart(); break;
                case "4": ViewCart(); break;
                case "5": UpdateCart(); break;
                case "6": Checkout(); break;
                case "7": ViewWalletBalance(); break;
                case "8": AddWalletFunds(); break;
                case "9": ViewOrderHistory(); break;
                case "10": TrackOrders(); break;
                case "11": ReviewProduct(); break;
                case "12":
                    Program.CurrentUser = null;
                    ConsoleHelper.PrintSuccess("Logged out successfully.");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    return;
                default:
                    ConsoleHelper.PrintError("Invalid option. Please try again.");
                    Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
                    break;
            }
        }
    }

    #region Shopping

    private void BrowseProducts()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Browse Products");
        var products = _productService.GetAllProducts().ToList();
        ConsoleHelper.PrintProductTable(products);

        if (!products.Any())
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintPrompt("Enter Product ID to view reviews (or press Enter to go back): ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        if (!int.TryParse(input, out var productId))
        {
            ConsoleHelper.PrintError("Invalid Product ID.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            var product = _productService.GetProductById(productId);
            Console.Clear();
            ConsoleHelper.PrintHeader($"Reviews for {product.Name}");
            SalesReportPrinter.PrintProductReviews(_reviewService.GetProductReviews(productId));
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        ConsoleHelper.PressAnyKey();
    }

    private void SearchProducts()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Search Products");
        ConsoleHelper.PrintMenuOption("1", "Search by Name");
        ConsoleHelper.PrintMenuOption("2", "Search by Category");
        Console.WriteLine();
        ConsoleHelper.PrintPrompt("Select search type: ");

        var choice = Console.ReadLine()?.Trim();
        IEnumerable<Product> results;

        if (choice == "1")
        {
            var searchTerm = ConsoleHelper.ReadNonEmptyString("  Enter product name: ");
            results = _productService.SearchByName(searchTerm);
        }
        else if (choice == "2")
        {
            var searchTerm = ConsoleHelper.ReadNonEmptyString("  Enter category: ");
            results = _productService.SearchByCategory(searchTerm);
        }
        else
        {
            ConsoleHelper.PrintError("Invalid option.");
            ConsoleHelper.PressAnyKey();
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintProductTable(results);
        ConsoleHelper.PressAnyKey();
    }

    private void AddProductToCart()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Add Product to Cart");
        ConsoleHelper.PrintProductTable(_productService.GetAllProducts());
        Console.WriteLine();

        if (!ConsoleHelper.TryReadInt("  Enter Product ID: ", out var productId))
        {
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        if (!ConsoleHelper.TryReadInt("  Enter Quantity: ", out var quantity))
        {
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            _cartService.AddToCart(_customer, new AddToCartRequest { ProductId = productId, Quantity = quantity });
            ConsoleHelper.PrintSuccess("Product added to cart successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }

    #endregion

    #region Cart & Checkout

    private void ViewCart()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Your Cart");
        ConsoleHelper.PrintCartTable(_cartService.GetCartItems(_customer), _cartService.GetCartTotal(_customer));
        ConsoleHelper.PressAnyKey();
    }

    private void UpdateCart()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Update Cart");
        var items = _cartService.GetCartItems(_customer);
        ConsoleHelper.PrintCartTable(items, _cartService.GetCartTotal(_customer));

        if (!items.Any())
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        Console.WriteLine();
        if (!ConsoleHelper.TryReadInt("  Enter Product ID to update: ", out var productId))
        {
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        if (!ConsoleHelper.TryReadInt("  Enter new quantity (0 to remove): ", out var newQuantity))
        {
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            _cartService.UpdateCartItem(_customer, new UpdateCartItemRequest { ProductId = productId, NewQuantity = newQuantity });
            ConsoleHelper.PrintSuccess(newQuantity == 0 ? "Product removed from cart." : "Cart updated successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }

    private void Checkout()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Checkout");

        var items = _cartService.GetCartItems(_customer);
        var total = _cartService.GetCartTotal(_customer);
        ConsoleHelper.PrintCartTable(items, total);

        if (!items.Any())
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintInfo($"Wallet Balance: {_customer.Balance:F2}");
        ConsoleHelper.PrintInfo($"Order Total:    {total:F2}");
        Console.WriteLine();
        ConsoleHelper.PrintPrompt("Confirm order? (y/n): ");

        if (Console.ReadLine()?.Trim().ToLower() != "y")
        {
            ConsoleHelper.PrintWarning("Checkout cancelled.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            var order = _orderService.PlaceOrder(_customer);
            ConsoleHelper.PrintSuccess($"Order #{order.Id} placed successfully! Status: {order.Status}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        ConsoleHelper.PressAnyKey();
    }

    #endregion

    #region Wallet

    private void ViewWalletBalance()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Wallet Balance");
        ConsoleHelper.PrintInfo($"Current Balance: {PaymentService.GetBalance(_customer):F2}");
        ConsoleHelper.PressAnyKey();
    }

    private void AddWalletFunds()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Add Wallet Funds");
        ConsoleHelper.PrintInfo($"Current Balance: {PaymentService.GetBalance(_customer):F2}");
        Console.WriteLine();

        var amount = ConsoleHelper.ReadPositiveDecimal("  Enter amount to add: ");

        _paymentService.AddFunds(_customer, new AddFundsRequest { Amount = amount });
        ConsoleHelper.PrintSuccess($"Funds added successfully. New Balance: {PaymentService.GetBalance(_customer):F2}");
        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }

    #endregion
}
