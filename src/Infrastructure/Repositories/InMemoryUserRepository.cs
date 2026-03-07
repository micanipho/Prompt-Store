namespace Infrastructure.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public InMemoryUserRepository()
    {
        _users.Add(new Administrator("admin", "admin123"));
    }

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public User? GetUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public bool UsernameExists(string username)
    {
        return _users.Any(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}