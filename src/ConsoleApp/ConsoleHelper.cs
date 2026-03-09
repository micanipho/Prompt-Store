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
    internal static void WriteColored(string text, ConsoleColor color)
    {
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = saved;
    }

    /// <summary>Finds the project root directory by looking for the solution file.</summary>
    public static string GetProjectRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (current.GetFiles("OnlineShopping.slnx").Any())
                return current.FullName;
            current = current.Parent;
        }
        return AppContext.BaseDirectory; // Fallback
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
            PrintTableRow(ProductTableWidth, $"{product.Id,-5} {product.Name,-25} {product.Category,-15} R{product.Price,10:F2} {product.Stock,6}");
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
            PrintTableRow(CartTableWidth, $"{item.Product.Id,-5} {item.Product.Name,-25} {item.Product.Price,10:F2} {item.Quantity,5} R{item.Product.Price * item.Quantity,10:F2}");
        PrintTableMid(CartTableWidth);
        PrintTableRow(CartTableWidth, $"{"Total:",-49} R{total,10:F2}");
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
            PrintTableRow(OrderTableWidth, $"{order.Id,-5} {order.PlacedAt,-22:yyyy-MM-dd HH:mm:ss} {order.Status,-12} {order.Items.Count,6} R{order.Total,10:F2}");
        PrintTableBot(OrderTableWidth);
    }

    private const int OrderDetailsTableWidth = 59;

    /// <summary>Prints the full details of a single order including all line items.</summary>
    public static void PrintOrderDetails(Order order)
    {
        PrintInfo($"Order #{order.Id}  \u2502  Placed: {order.PlacedAt:yyyy-MM-dd HH:mm:ss}  \u2502  Status: {order.Status}");
        if (!string.IsNullOrEmpty(order.DiscountApplied) && order.DiscountApplied != "None")
            PrintInfo($"Discount: {order.DiscountApplied}");
            
        Console.WriteLine();
        PrintTableTop(OrderDetailsTableWidth);
        PrintTableRow(OrderDetailsTableWidth, $"{"Product",-30} {"Unit Price",10} {"Qty",5} {"Subtotal",10}");
        PrintTableMid(OrderDetailsTableWidth);
        foreach (var item in order.Items)
            PrintTableRow(OrderDetailsTableWidth, $"{item.Product.Name,-30} R{item.UnitPrice,10:F2} {item.Quantity,5} R{item.UnitPrice * item.Quantity,10:F2}");
        PrintTableMid(OrderDetailsTableWidth);
        PrintTableRow(OrderDetailsTableWidth, $"{"Total:",-47} R{order.Total,10:F2}");
        PrintTableBot(OrderDetailsTableWidth);
    }

    /// <summary>Prints the top border of a table.</summary>
    internal static void PrintTableTop(int innerWidth)
    {
        WriteColored($"  \u250c{new string('\u2500', innerWidth + 2)}\u2510", ConsoleColor.DarkGray);
    }

    /// <summary>Prints a middle separator row in a table.</summary>
    internal static void PrintTableMid(int innerWidth)
    {
        WriteColored($"  \u251c{new string('\u2500', innerWidth + 2)}\u2524", ConsoleColor.DarkGray);
    }

    /// <summary>Prints the bottom border of a table.</summary>
    internal static void PrintTableBot(int innerWidth)
    {
        WriteColored($"  \u2514{new string('\u2500', innerWidth + 2)}\u2518", ConsoleColor.DarkGray);
    }

    /// <summary>Prints a single content row inside a table.</summary>
    internal static void PrintTableRow(int innerWidth, string content)
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

    #region Input Helpers

    /// <summary>Attempts to read and parse an integer from the console. Returns false if the input is not a valid integer.</summary>
    public static bool TryReadInt(string prompt, out int value)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine()?.Trim(), out value))
            return true;

        PrintError("Invalid number.");
        return false;
    }

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
