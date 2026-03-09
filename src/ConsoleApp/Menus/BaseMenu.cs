using ConsoleApp.Menus.Commands;

namespace ConsoleApp.Menus;

/// <summary>Abstract base class for all menus that handle command-based interactions.</summary>
public abstract class BaseMenu
{
    protected readonly Dictionary<string, IMenuCommand> _commands = [];
    protected abstract string Header { get; }

    /// <summary>Displays the menu and processes user input in a loop.</summary>
    public virtual void Show()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader($"{Header} \u2502 {Program.CurrentUser?.UserName}");

            PrintMenuContent();

            ConsoleHelper.PrintSeparator();
            ConsoleHelper.PrintMenuOption("0", "Logout/Back");
            Console.WriteLine();
            ConsoleHelper.PrintPrompt("Select an option: ");

            var input = Console.ReadLine()?.Trim();
            if (input == "0") return;

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

    /// <summary>Override to print category headers and menu options.</summary>
    protected abstract void PrintMenuContent();

    protected void AddCommand(string key, string description, Action action)
    {
        _commands[key] = new DelegateCommand(description, action);
    }
}
