namespace ConsoleApp;

/// <summary>Shared console utility methods used across menus.</summary>
internal static class ConsoleHelper
{
    /// <summary>Milliseconds to pause after displaying a feedback message before clearing the screen.</summary>
    public const int FeedbackDelayMs = 1500;

    #region UI Primitives

    /// <summary>Prints a styled section header with box-drawing borders.</summary>
    public static void PrintHeader(string title)
    {
        var inner = $"  {title}  ";
        var width = inner.Length;
        var top = $"\u250c{new string('\u2500', width)}\u2510";
        var mid = $"\u2502{inner}\u2502";
        var bot = $"\u2514{new string('\u2500', width)}\u2518";

        WriteColored(top, ConsoleColor.Cyan);
        WriteColored(mid, ConsoleColor.Cyan);
        WriteColored(bot, ConsoleColor.Cyan);
        Console.WriteLine();
    }

    /// <summary>Prints a sub-section header with a subtle line.</summary>
    public static void PrintSubHeader(string title)
    {
        WriteColored($"  \u2500\u2500 {title} \u2500\u2500", ConsoleColor.DarkCyan);
    }

    /// <summary>Prints a success message with a green checkmark.</summary>
    public static void PrintSuccess(string message)
    {
        WriteColored($"  [\u2713] {message}", ConsoleColor.Green);
    }

    /// <summary>Prints an error message with a red cross.</summary>
    public static void PrintError(string message)
    {
        WriteColored($"  [\u2717] {message}", ConsoleColor.Red);
    }

    /// <summary>Prints a warning message in yellow.</summary>
    public static void PrintWarning(string message)
    {
        WriteColored($"  [!] {message}", ConsoleColor.Yellow);
    }

    /// <summary>Prints an informational message in dark cyan.</summary>
    public static void PrintInfo(string message)
    {
        WriteColored($"  {message}", ConsoleColor.DarkCyan);
    }

    /// <summary>Prints the application welcome banner.</summary>
    public static void PrintBanner()
    {
        var banner = """

             ____                            _     ____  _
            |  _ \ _ __ ___  _ __ ___  _ __ | |_  / ___|| |_ ___  _ __ ___
            | |_) | '__/ _ \| '_ ` _ \| '_ \| __| \___ \| __/ _ \| '__/ _ \
            |  __/| | | (_) | | | | | | |_) | |_   ___) | || (_) | | |  __/
            |_|   |_|  \___/|_| |_| |_| .__/ \__| |____/ \__\___/|_|  \___|
                                       |_|
            """;
        WriteColored(banner, ConsoleColor.Cyan);
        WriteColored("            Your one-stop online shopping experience", ConsoleColor.DarkGray);
        Console.WriteLine();
    }

    /// <summary>Prints a styled menu option line.</summary>
    public static void PrintMenuOption(string number, string label)
    {
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"   [{number}]");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  {label}");
        Console.ForegroundColor = saved;
    }

    /// <summary>Prints a thin separator line.</summary>
    public static void PrintSeparator(int width = 55)
    {
        WriteColored($"  {new string('\u2500', width)}", ConsoleColor.DarkGray);
    }

    /// <summary>Prints a "press any key" prompt with subtle styling.</summary>
    public static void PressAnyKey(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        WriteColored($"  {message}", ConsoleColor.DarkGray);
        Console.ReadKey(true);
    }

    /// <summary>Prints a styled input prompt.</summary>
    public static void PrintPrompt(string message)
    {
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"  \u25b6 ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(message);
        Console.ForegroundColor = saved;
    }

    /// <summary>Writes colored text to the console and resets the color.</summary>
    private static void WriteColored(string text, ConsoleColor color)
    {
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = saved;
    }

    #endregion

    #region Tables

    private const int ProductTableWidth = 67;

    /// <summary>Prints a formatted table of products. Displays a "no products" message if the list is empty.</summary>
    public static void PrintProductTable(IEnumerable<Product> products)
    {
        var list = products.ToList();
        if (!list.Any())
        {
            PrintWarning("No products found.");
            return;
        }

        PrintTableTop(ProductTableWidth);
        PrintTableRow(ProductTableWidth, $"{"ID",-5} {"Name",-25} {"Category",-15} {"Price",10} {"Stock",6}");
        PrintTableMid(ProductTableWidth);
        foreach (var product in list)
            PrintTableRow(ProductTableWidth, $"{product.Id,-5} {product.Name,-25} {product.Category,-15} {product.Price,10:F2} {product.Stock,6}");
        PrintTableBot(ProductTableWidth);
    }

    private const int CartTableWidth = 61;

    /// <summary>Prints a formatted table of cart items with subtotals and a grand total.</summary>
    public static void PrintCartTable(IEnumerable<CartItem> items, decimal total)
    {
        var list = items.ToList();
        if (!list.Any())
        {
            PrintWarning("Your cart is empty.");
            return;
        }

        PrintTableTop(CartTableWidth);
        PrintTableRow(CartTableWidth, $"{"ID",-5} {"Product",-25} {"Price",10} {"Qty",5} {"Subtotal",10}");
        PrintTableMid(CartTableWidth);
        foreach (var item in list)
            PrintTableRow(CartTableWidth, $"{item.Product.Id,-5} {item.Product.Name,-25} {item.Product.Price,10:F2} {item.Quantity,5} {item.Product.Price * item.Quantity,10:F2}");
        PrintTableMid(CartTableWidth);
        PrintTableRow(CartTableWidth, $"{"Total:",-49} {total,10:F2}");
        PrintTableBot(CartTableWidth);
    }

    private const int OrderTableWidth = 61;

    /// <summary>Prints a summary table of orders. Displays a "no orders" message if the list is empty.</summary>
    public static void PrintOrderTable(IEnumerable<Order> orders)
    {
        var list = orders.ToList();
        if (!list.Any())
        {
            PrintWarning("No orders found.");
            return;
        }

        PrintTableTop(OrderTableWidth);
        PrintTableRow(OrderTableWidth, $"{"ID",-5} {"Placed At",-22} {"Status",-12} {"Items",6} {"Total",10}");
        PrintTableMid(OrderTableWidth);
        foreach (var order in list)
            PrintTableRow(OrderTableWidth, $"{order.Id,-5} {order.PlacedAt,-22:yyyy-MM-dd HH:mm:ss} {order.Status,-12} {order.Items.Count,6} {order.Total,10:F2}");
        PrintTableBot(OrderTableWidth);
    }

    /// <summary>Prints the full details of a single order including all line items.</summary>
    public static void PrintOrderDetails(Order order)
    {
        var width = 59;
        PrintInfo($"Order #{order.Id}  \u2502  Placed: {order.PlacedAt:yyyy-MM-dd HH:mm:ss}  \u2502  Status: {order.Status}");
        Console.WriteLine();
        PrintTableTop(width);
        PrintTableRow(width, $"{"Product",-30} {"Unit Price",10} {"Qty",5} {"Subtotal",10}");
        PrintTableMid(width);
        foreach (var item in order.Items)
            PrintTableRow(width, $"{item.Product.Name,-30} {item.UnitPrice,10:F2} {item.Quantity,5} {item.UnitPrice * item.Quantity,10:F2}");
        PrintTableMid(width);
        PrintTableRow(width, $"{"Total:",-47} {order.Total,10:F2}");
        PrintTableBot(width);
    }

    private static void PrintTableTop(int innerWidth)
    {
        WriteColored($"  \u250c{new string('\u2500', innerWidth + 2)}\u2510", ConsoleColor.DarkGray);
    }

    private static void PrintTableMid(int innerWidth)
    {
        WriteColored($"  \u251c{new string('\u2500', innerWidth + 2)}\u2524", ConsoleColor.DarkGray);
    }

    private static void PrintTableBot(int innerWidth)
    {
        WriteColored($"  \u2514{new string('\u2500', innerWidth + 2)}\u2518", ConsoleColor.DarkGray);
    }

    private static void PrintTableRow(int innerWidth, string content)
    {
        var padded = content.PadRight(innerWidth);
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  \u2502 ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(padded);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(" \u2502");
        Console.ForegroundColor = saved;
    }

    #endregion

    #region Reviews

    /// <summary>Prints all reviews for a product, including average rating.</summary>
    public static void PrintProductReviews(IEnumerable<Review> reviews)
    {
        var list = reviews.ToList();
        if (!list.Any())
        {
            PrintWarning("No reviews yet.");
            return;
        }

        var average = list.Average(r => r.Rating);
        WriteColored($"  Average Rating: {average:F1}/5 ({list.Count} review{(list.Count == 1 ? "" : "s")})", ConsoleColor.Yellow);
        Console.WriteLine();

        foreach (var review in list)
        {
            var stars = new string('\u2605', review.Rating) + new string('\u2606', 5 - review.Rating);
            var saved = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"    {stars}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  ({review.Rating}/5)");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"    \"{review.Comment}\"");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"    \u2014 Customer #{review.CustomerId} on {review.CreatedAt:yyyy-MM-dd}");
            Console.ForegroundColor = saved;
            Console.WriteLine();
        }
    }

    #endregion

    #region Sales Report

    private const int StatusTableWidth = 37;
    private const int TopProductsTableWidth = 54;
    private const int DailySalesTableWidth = 36;

    /// <summary>Prints a full sales report including totals, order status breakdown, top products, and daily sales.</summary>
    public static void PrintSalesReport(
        int totalOrders,
        decimal totalRevenue,
        decimal averageOrderValue,
        IEnumerable<OrderStatusSummary> statusSummaries,
        IEnumerable<ProductSalesSummary> topProducts,
        IEnumerable<DailySalesSummary> dailySales)
    {
        PrintOverview(totalOrders, totalRevenue, averageOrderValue);
        PrintStatusBreakdown(statusSummaries);
        PrintTopSellingProducts(topProducts);
        PrintDailySalesBreakdown(dailySales);
    }

    private static void PrintOverview(int totalOrders, decimal totalRevenue, decimal averageOrderValue)
    {
        PrintSubHeader("Overview");
        Console.WriteLine($"    Total Orders:         {totalOrders}");
        Console.WriteLine($"    Total Revenue:        {totalRevenue:F2}");
        Console.WriteLine($"    Average Order Value:  {averageOrderValue:F2}");
        Console.WriteLine();
    }

    private static void PrintStatusBreakdown(IEnumerable<OrderStatusSummary> statusSummaries)
    {
        PrintSubHeader("Orders by Status");
        var statusList = statusSummaries.ToList();
        if (statusList.Any())
        {
            PrintTableTop(StatusTableWidth);
            PrintTableRow(StatusTableWidth, $"{"Status",-15} {"Count",8} {"Revenue",12}");
            PrintTableMid(StatusTableWidth);
            foreach (var status in statusList)
                PrintTableRow(StatusTableWidth, $"{status.Status,-15} {status.Count,8} {status.Total,12:F2}");
            PrintTableBot(StatusTableWidth);
        }
        else
        {
            PrintWarning("No orders found.");
        }
        Console.WriteLine();
    }

    private static void PrintTopSellingProducts(IEnumerable<ProductSalesSummary> topProducts)
    {
        PrintSubHeader("Top Selling Products");
        var productList = topProducts.ToList();
        if (productList.Any())
        {
            PrintTableTop(TopProductsTableWidth);
            PrintTableRow(TopProductsTableWidth, $"{"ID",-5} {"Product",-25} {"Qty Sold",10} {"Revenue",12}");
            PrintTableMid(TopProductsTableWidth);
            foreach (var product in productList)
                PrintTableRow(TopProductsTableWidth, $"{product.ProductId,-5} {product.ProductName,-25} {product.TotalQuantitySold,10} {product.TotalRevenue,12:F2}");
            PrintTableBot(TopProductsTableWidth);
        }
        else
        {
            PrintWarning("No sales data available.");
        }
        Console.WriteLine();
    }

    private static void PrintDailySalesBreakdown(IEnumerable<DailySalesSummary> dailySales)
    {
        PrintSubHeader("Daily Sales");
        var dailyList = dailySales.ToList();
        if (dailyList.Any())
        {
            PrintTableTop(DailySalesTableWidth);
            PrintTableRow(DailySalesTableWidth, $"{"Date",-14} {"Orders",8} {"Revenue",12}");
            PrintTableMid(DailySalesTableWidth);
            foreach (var day in dailyList)
                PrintTableRow(DailySalesTableWidth, $"{day.Date:yyyy-MM-dd}     {day.OrderCount,8} {day.Revenue,12:F2}");
            PrintTableBot(DailySalesTableWidth);
        }
        else
        {
            PrintWarning("No sales data available.");
        }
    }

    #endregion

    #region Input Helpers

    /// <summary>Reads a non-empty, trimmed string from the console. Re-prompts until valid input is provided.</summary>
    public static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
                return input;
            PrintWarning("Input cannot be empty. Please try again.");
        }
    }

    /// <summary>Reads and parses an integer from the console. Re-prompts until a valid integer is entered.</summary>
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine()?.Trim(), out var value))
                return value;
            PrintWarning("Invalid number. Please enter a valid whole number.");
        }
    }

    /// <summary>Reads and parses a positive integer (greater than zero). Re-prompts until valid.</summary>
    public static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            var value = ReadInt(prompt);
            if (value > 0)
                return value;
            PrintWarning("Value must be greater than zero.");
        }
    }

    /// <summary>Reads and parses a decimal from the console. Re-prompts until a valid decimal is entered.</summary>
    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine()?.Trim(), out var value))
                return value;
            PrintWarning("Invalid number. Please enter a valid amount.");
        }
    }

    /// <summary>Reads and parses a positive decimal (greater than zero). Re-prompts until valid.</summary>
    public static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            var value = ReadDecimal(prompt);
            if (value > 0)
                return value;
            PrintWarning("Value must be greater than zero.");
        }
    }

    /// <summary>Reads a password from the console, masking input with asterisks.</summary>
    public static string ReadPassword()
    {
        var password = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
                break;
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Length--;
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return password.ToString();
    }

    #endregion
}
