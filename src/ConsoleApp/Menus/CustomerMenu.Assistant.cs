namespace ConsoleApp.Menus;

/// <summary>AI shopping assistant conversation loop for the customer menu.</summary>
public partial class CustomerMenu
{
    private void StartShoppingAssistant()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("AI Shopping Assistant");
        ConsoleHelper.WriteColored("    Powered by Claude AI \u2502 Your intelligent shopping companion", ConsoleColor.Magenta);
        Console.WriteLine();
        ConsoleHelper.PrintInfo("Ask me about products, recommendations, orders, cart suggestions, or price comparisons.");
        ConsoleHelper.PrintInfo("Type 'exit' or 'quit' to return to the Customer Menu.");
        Console.WriteLine();

        var conversationHistory = new List<ChatMessage>();

        while (true)
        {
            ConsoleHelper.PrintPrompt("You: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                return;

            Console.WriteLine();
            Console.Write("    [Assistant is thinking...]");

            try
            {
                var response = _assistantService.ChatAsync(_customer, input, conversationHistory)
                                                .GetAwaiter().GetResult();

                // Overwrite the "thinking" line
                Console.Write("\r" + new string(' ', 40) + "\r");
                PrintAssistantResponse(response);
            }
            catch (Exception ex)
            {
                Console.Write("\r" + new string(' ', 40) + "\r");
                ConsoleHelper.PrintError($"Assistant unavailable: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private static void PrintAssistantResponse(string response)
    {
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("    Assistant: ");
        Console.ForegroundColor = ConsoleColor.White;

        const int indent = 18; // length of "    Assistant: "
        const int maxWidth = 82;

        var lines = response.Replace("\r\n", "\n").Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var prefix = i == 0 ? string.Empty : new string(' ', indent);
            var words = lines[i].Split(' ');
            var lineLen = i == 0 ? indent : 0;

            Console.Write(prefix);
            foreach (var word in words)
            {
                if (lineLen + word.Length + 1 > maxWidth && lineLen > indent)
                {
                    Console.WriteLine();
                    Console.Write(new string(' ', indent));
                    lineLen = indent;
                }
                Console.Write(word + " ");
                lineLen += word.Length + 1;
            }
            Console.WriteLine();
        }

        Console.ForegroundColor = saved;
    }
}
