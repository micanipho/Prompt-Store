namespace ConsoleApp.Menus.Commands;

/// <summary>Simple command that delegates execution to an Action.</summary>
public class DelegateCommand(string description, Action executeAction) : IMenuCommand
{
    public string Description { get; } = description;
    private readonly Action _executeAction = executeAction;

    public void Execute() => _executeAction();
}
