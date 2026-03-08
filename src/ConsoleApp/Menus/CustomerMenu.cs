namespace ConsoleApp.Menus;

/// <summary>Displays the customer menu with shopping, order, and wallet options.</summary>
public class CustomerMenu
{
    private readonly Customer _customer;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly ReviewService _reviewService;

    public CustomerMenu(Customer customer, ProductService productService, CartService cartService, OrderService orderService, ReviewService reviewService)
    {
        _customer = customer;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _reviewService = reviewService;
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
            ConsoleHelper.PrintProductReviews(_reviewService.GetProductReviews(productId));
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
        Console.Write("  Enter Product ID: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var productId))
        {
            ConsoleHelper.PrintError("Invalid Product ID.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        Console.Write("  Enter Quantity: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var quantity))
        {
            ConsoleHelper.PrintError("Invalid quantity.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            _cartService.AddToCart(_customer, new AddToCartRequest
            {
                ProductId = productId,
                Quantity = quantity
            });
            ConsoleHelper.PrintSuccess("Product added to cart successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }

    private void ViewCart()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Your Cart");
        var items = _cartService.GetCartItems(_customer);
        var total = _cartService.GetCartTotal(_customer);
        ConsoleHelper.PrintCartTable(items, total);
        ConsoleHelper.PressAnyKey();
    }

    private void UpdateCart()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Update Cart");
        var items = _cartService.GetCartItems(_customer);
        var total = _cartService.GetCartTotal(_customer);
        ConsoleHelper.PrintCartTable(items, total);

        if (!items.Any())
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        Console.WriteLine();
        Console.Write("  Enter Product ID to update: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var productId))
        {
            ConsoleHelper.PrintError("Invalid Product ID.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        Console.Write("  Enter new quantity (0 to remove): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var newQuantity))
        {
            ConsoleHelper.PrintError("Invalid quantity.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        try
        {
            _cartService.UpdateCartItem(_customer, new UpdateCartItemRequest
            {
                ProductId = productId,
                NewQuantity = newQuantity
            });
            ConsoleHelper.PrintSuccess(newQuantity == 0
                ? "Product removed from cart."
                : "Cart updated successfully.");
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

        PaymentService.AddFunds(_customer, new AddFundsRequest { Amount = amount });
        ConsoleHelper.PrintSuccess($"Funds added successfully. New Balance: {PaymentService.GetBalance(_customer):F2}");
        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }

    private void ViewOrderHistory()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Order History");
        ConsoleHelper.PrintOrderTable(_orderService.GetOrderHistory(_customer));
        ConsoleHelper.PressAnyKey();
    }

    private void TrackOrders()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Track Orders");

        var orders = _orderService.GetOrderHistory(_customer);
        ConsoleHelper.PrintOrderTable(orders);

        if (!orders.Any())
        {
            ConsoleHelper.PressAnyKey();
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintPrompt("Enter Order ID to view details (or press Enter to go back): ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        if (!int.TryParse(input, out var orderId))
        {
            ConsoleHelper.PrintError("Invalid Order ID.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        if (!_customer.OrderHistory.Any(o => o.Id == orderId))
        {
            ConsoleHelper.PrintWarning("Order not found in your history.");
        }
        else
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                Console.Clear();
                ConsoleHelper.PrintHeader("Order Details");
                ConsoleHelper.PrintOrderDetails(order);
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError(ex.Message);
            }
        }

        ConsoleHelper.PressAnyKey();
    }

    private void ReviewProduct()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("Review a Product");

        var purchasedProducts = _customer.OrderHistory
            .SelectMany(order => order.Items)
            .Select(item => item.Product)
            .DistinctBy(product => product.Id)
            .ToList();

        if (!purchasedProducts.Any())
        {
            ConsoleHelper.PrintWarning("You have no purchased products to review.");
            ConsoleHelper.PressAnyKey();
            return;
        }

        ConsoleHelper.PrintSubHeader("Your Purchased Products");
        Console.WriteLine();
        ConsoleHelper.PrintProductTable(purchasedProducts);

        Console.WriteLine();
        Console.Write("  Enter Product ID to review: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var productId))
        {
            ConsoleHelper.PrintError("Invalid Product ID.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        if (!purchasedProducts.Any(p => p.Id == productId))
        {
            ConsoleHelper.PrintError("You have not purchased this product.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        Console.Write("  Enter Rating (1-5): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var rating) || rating < 1 || rating > 5)
        {
            ConsoleHelper.PrintError("Invalid rating. Must be a whole number between 1 and 5.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        var comment = ConsoleHelper.ReadNonEmptyString("  Enter your review comment: ");

        try
        {
            _reviewService.SubmitReview(_customer, new SubmitReviewRequest
            {
                ProductId = productId,
                Rating = rating,
                Comment = comment
            });
            ConsoleHelper.PrintSuccess("Review submitted successfully!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }
}
