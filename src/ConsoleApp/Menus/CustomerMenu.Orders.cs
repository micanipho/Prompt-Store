namespace ConsoleApp.Menus;

/// <summary>Order tracking, history, and product review actions for the customer menu.</summary>
public partial class CustomerMenu
{
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

        ShowOrderDetails(orderId);
        ConsoleHelper.PressAnyKey();
    }

    private void ShowOrderDetails(int orderId)
    {
        if (!_customer.OrderHistory.Any(o => o.Id == orderId))
        {
            ConsoleHelper.PrintWarning("Order not found in your history.");
            return;
        }

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

        if (!ConsoleHelper.TryReadInt("  Enter Product ID to review: ", out var productId))
        {
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        if (!purchasedProducts.Any(p => p.Id == productId))
        {
            ConsoleHelper.PrintError("You have not purchased this product.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        Console.Write($"  Enter Rating ({Review.MinRating}-{Review.MaxRating}): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var rating) || rating < Review.MinRating || rating > Review.MaxRating)
        {
            ConsoleHelper.PrintError($"Invalid rating. Must be a whole number between {Review.MinRating} and {Review.MaxRating}.");
            Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
            return;
        }

        var comment = ConsoleHelper.ReadNonEmptyString("  Enter your review comment: ");

        try
        {
            _reviewService.SubmitReview(_customer, new SubmitReviewRequest { ProductId = productId, Rating = rating, Comment = comment });
            ConsoleHelper.PrintSuccess("Review submitted successfully!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError(ex.Message);
        }

        Thread.Sleep(ConsoleHelper.FeedbackDelayMs);
    }
}
