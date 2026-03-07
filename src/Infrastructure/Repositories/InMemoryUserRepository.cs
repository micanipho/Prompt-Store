namespace Infrastructure.Repositories;

/// <summary>In-memory implementation of IUserRepository. Stores users in a List for the lifetime of the application.</summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public InMemoryUserRepository()
    {
        _users.Add(new Administrator("admin", "admin123"));
    }

    /// <summary>Adds a new user to the in-memory store.</summary>
    public void AddUser(User user)
    {
        _users.Add(user);
    }

    /// <summary>Returns the user matching the given username, or null if not found.</summary>
    public User? GetUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Returns true if a user with the given username already exists.</summary>
    public bool UsernameExists(string username)
    {
        return _users.Any(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}