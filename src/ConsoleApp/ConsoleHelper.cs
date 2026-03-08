namespace ConsoleApp;

/// <summary>Shared console utility methods used across menus.</summary>
internal static class ConsoleHelper
{
    /// <summary>Milliseconds to pause after displaying a feedback message before clearing the screen.</summary>
    public const int FeedbackDelayMs = 1500;

    private const int ProductTableWidth = 65;

    /// <summary>Prints a formatted table of products. Displays a "no products" message if the list is empty.</summary>
    public static void PrintProductTable(IEnumerable<Product> products)
    {
        var list = products.ToList();
        if (!list.Any())
        {
            Console.WriteLine("No products found.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Name",-25} {"Category",-15} {"Price",10} {"Stock",6}");
        Console.WriteLine(new string('-', ProductTableWidth));
        foreach (var p in list)
            Console.WriteLine($"{p.Id,-5} {p.Name,-25} {p.Category,-15} {p.Price,10:F2} {p.Stock,6}");
    }

    private const int CartTableWidth = 65;

    /// <summary>Prints a formatted table of cart items with subtotals and a grand total.</summary>
    public static void PrintCartTable(IEnumerable<CartItem> items, decimal total)
    {
        var list = items.ToList();
        if (!list.Any())
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Product",-25} {"Price",10} {"Qty",5} {"Subtotal",10}");
        Console.WriteLine(new string('-', CartTableWidth));
        foreach (var item in list)
            Console.WriteLine($"{item.Product.Id,-5} {item.Product.Name,-25} {item.Product.Price,10:F2} {item.Quantity,5} {item.Product.Price * item.Quantity,10:F2}");
        Console.WriteLine(new string('-', CartTableWidth));
        Console.WriteLine($"{"Total:",-47} {total,10:F2}");
    }

    private const int OrderTableWidth = 65;

    /// <summary>Prints a summary table of orders. Displays a "no orders" message if the list is empty.</summary>
    public static void PrintOrderTable(IEnumerable<Order> orders)
    {
        var list = orders.ToList();
        if (!list.Any())
        {
            Console.WriteLine("No orders found.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Placed At",-22} {"Status",-12} {"Items",6} {"Total",10}");
        Console.WriteLine(new string('-', OrderTableWidth));
        foreach (var o in list)
            Console.WriteLine($"{o.Id,-5} {o.PlacedAt,-22:yyyy-MM-dd HH:mm:ss} {o.Status,-12} {o.Items.Count,6} {o.Total,10:F2}");
    }

    /// <summary>Prints the full details of a single order including all line items.</summary>
    public static void PrintOrderDetails(Order order)
    {
        Console.WriteLine($"Order #{order.Id}  |  Placed: {order.PlacedAt:yyyy-MM-dd HH:mm:ss}  |  Status: {order.Status}");
        Console.WriteLine(new string('-', OrderTableWidth));
        Console.WriteLine($"{"Product",-30} {"Unit Price",10} {"Qty",5} {"Subtotal",10}");
        Console.WriteLine(new string('-', OrderTableWidth));
        foreach (var item in order.Items)
            Console.WriteLine($"{item.Product.Name,-30} {item.UnitPrice,10:F2} {item.Quantity,5} {item.UnitPrice * item.Quantity,10:F2}");
        Console.WriteLine(new string('-', OrderTableWidth));
        Console.WriteLine($"{"Total:",-47} {order.Total,10:F2}");
    }

    /// <summary>Prints a full sales report including totals, order status breakdown, top products, and daily sales.</summary>
    public static void PrintSalesReport(
        int totalOrders,
        decimal totalRevenue,
        decimal averageOrderValue,
        IEnumerable<OrderStatusSummary> statusSummaries,
        IEnumerable<ProductSalesSummary> topProducts,
        IEnumerable<DailySalesSummary> dailySales)
    {
        Console.WriteLine("--- Overview ---");
        Console.WriteLine($"  Total Orders:         {totalOrders}");
        Console.WriteLine($"  Total Revenue:        {totalRevenue:F2}");
        Console.WriteLine($"  Average Order Value:  {averageOrderValue:F2}");

        Console.WriteLine();
        Console.WriteLine("--- Orders by Status ---");
        var statusList = statusSummaries.ToList();
        if (statusList.Any())
        {
            Console.WriteLine($"{"Status",-15} {"Count",8} {"Revenue",12}");
            Console.WriteLine(new string('-', 35));
            foreach (var s in statusList)
                Console.WriteLine($"{s.Status,-15} {s.Count,8} {s.Total,12:F2}");
        }
        else
        {
            Console.WriteLine("  No orders found.");
        }

        Console.WriteLine();
        Console.WriteLine("--- Top Selling Products ---");
        var productList = topProducts.ToList();
        if (productList.Any())
        {
            Console.WriteLine($"{"ID",-5} {"Product",-25} {"Qty Sold",10} {"Revenue",12}");
            Console.WriteLine(new string('-', 52));
            foreach (var p in productList)
                Console.WriteLine($"{p.ProductId,-5} {p.ProductName,-25} {p.TotalQuantitySold,10} {p.TotalRevenue,12:F2}");
        }
        else
        {
            Console.WriteLine("  No sales data available.");
        }

        Console.WriteLine();
        Console.WriteLine("--- Daily Sales ---");
        var dailyList = dailySales.ToList();
        if (dailyList.Any())
        {
            Console.WriteLine($"{"Date",-14} {"Orders",8} {"Revenue",12}");
            Console.WriteLine(new string('-', 34));
            foreach (var d in dailyList)
                Console.WriteLine($"{d.Date:yyyy-MM-dd}     {d.OrderCount,8} {d.Revenue,12:F2}");
        }
        else
        {
            Console.WriteLine("  No sales data available.");
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
}
