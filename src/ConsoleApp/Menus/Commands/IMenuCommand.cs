namespace ConsoleApp.Menus.Commands;

/// <summary>Defines the contract for a menu command.</summary>
public interface IMenuCommand
{
    /// <summary>The display name of the command in the menu.</summary>
    string Description { get; }

    /// <summary>Executes the command logic.</summary>
    void Execute();
}
