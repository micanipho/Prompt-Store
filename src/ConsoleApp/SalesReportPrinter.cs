namespace ConsoleApp;

/// <summary>Renders sales report tables, review displays, and analytics to the console.</summary>
internal static class SalesReportPrinter
{
    #region Reviews

    /// <summary>Prints all reviews for a product, including average rating.</summary>
    public static void PrintProductReviews(IEnumerable<Review> reviews)
    {
        var list = reviews.ToList();
        if (!list.Any())
        {
            ConsoleHelper.PrintWarning("No reviews yet.");
            return;
        }

        var average = list.Average(r => r.Rating);
        ConsoleHelper.WriteColored($"  Average Rating: {average:F1}/{Review.MaxRating} ({list.Count} review{(list.Count == 1 ? "" : "s")})", ConsoleColor.Yellow);
        Console.WriteLine();

        foreach (var review in list)
        {
            var stars = new string('\u2605', review.Rating) + new string('\u2606', Review.MaxRating - review.Rating);
            var saved = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"    {stars}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  ({review.Rating}/{Review.MaxRating})");
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
        ConsoleHelper.PrintSubHeader("Overview");
        Console.WriteLine($"    Total Orders:         {totalOrders}");
        Console.WriteLine($"    Total Revenue:        {totalRevenue:F2}");
        Console.WriteLine($"    Average Order Value:  {averageOrderValue:F2}");
        Console.WriteLine();
    }

    private static void PrintStatusBreakdown(IEnumerable<OrderStatusSummary> statusSummaries)
    {
        ConsoleHelper.PrintSubHeader("Orders by Status");
        var statusList = statusSummaries.ToList();
        if (statusList.Any())
        {
            ConsoleHelper.PrintTableTop(StatusTableWidth);
            ConsoleHelper.PrintTableRow(StatusTableWidth, $"{"Status",-15} {"Count",8} {"Revenue",12}");
            ConsoleHelper.PrintTableMid(StatusTableWidth);
            foreach (var status in statusList)
                ConsoleHelper.PrintTableRow(StatusTableWidth, $"{status.Status,-15} {status.Count,8} {status.Total,12:F2}");
            ConsoleHelper.PrintTableBot(StatusTableWidth);
        }
        else
        {
            ConsoleHelper.PrintWarning("No orders found.");
        }
        Console.WriteLine();
    }

    private static void PrintTopSellingProducts(IEnumerable<ProductSalesSummary> topProducts)
    {
        ConsoleHelper.PrintSubHeader("Top Selling Products");
        var productList = topProducts.ToList();
        if (productList.Any())
        {
            ConsoleHelper.PrintTableTop(TopProductsTableWidth);
            ConsoleHelper.PrintTableRow(TopProductsTableWidth, $"{"ID",-5} {"Product",-25} {"Qty Sold",10} {"Revenue",12}");
            ConsoleHelper.PrintTableMid(TopProductsTableWidth);
            foreach (var product in productList)
                ConsoleHelper.PrintTableRow(TopProductsTableWidth, $"{product.ProductId,-5} {product.ProductName,-25} {product.TotalQuantitySold,10} {product.TotalRevenue,12:F2}");
            ConsoleHelper.PrintTableBot(TopProductsTableWidth);
        }
        else
        {
            ConsoleHelper.PrintWarning("No sales data available.");
        }
        Console.WriteLine();
    }

    private static void PrintDailySalesBreakdown(IEnumerable<DailySalesSummary> dailySales)
    {
        ConsoleHelper.PrintSubHeader("Daily Sales");
        var dailyList = dailySales.ToList();
        if (dailyList.Any())
        {
            ConsoleHelper.PrintTableTop(DailySalesTableWidth);
            ConsoleHelper.PrintTableRow(DailySalesTableWidth, $"{"Date",-14} {"Orders",8} {"Revenue",12}");
            ConsoleHelper.PrintTableMid(DailySalesTableWidth);
            foreach (var day in dailyList)
                ConsoleHelper.PrintTableRow(DailySalesTableWidth, $"{day.Date:yyyy-MM-dd}     {day.OrderCount,8} {day.Revenue,12:F2}");
            ConsoleHelper.PrintTableBot(DailySalesTableWidth);
        }
        else
        {
            ConsoleHelper.PrintWarning("No sales data available.");
        }
    }

    #endregion
}
