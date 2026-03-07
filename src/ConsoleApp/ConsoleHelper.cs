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
