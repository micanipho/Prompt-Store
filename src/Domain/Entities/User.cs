namespace Domain.Entities;

/// <summary>Base class for all users. Provides shared identity and authentication properties.</summary>
public abstract class User
{
    private static int _nextId = 0;

    public int Id { get; private set; }
    public string UserName { get; protected set; }
    public UserRole Role { get; protected set; }
    public string Password { get; protected set; }

    public User(string userName, UserRole role, string password)
    {
        Id = _nextId++;
        UserName = userName;
        Role = role;
        Password = password;
    }
}