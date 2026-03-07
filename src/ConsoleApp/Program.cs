namespace ConsoleApp;

public class Program
{
    protected Program() { }

    internal static readonly User? CurrentUser = null;

    public static void Main(string[] args)
    {
        var userRepository = new InMemoryUserRepository();
        var authService = new AuthService(userRepository);
        var mainMenu = new MainMenu(authService);

        mainMenu.Show();
    }
}
