namespace Domain.Entities;

/// <summary>Base class for all users. Provides shared identity and authentication properties.</summary>
public abstract class User
{
    public virtual int Id { get; protected set; }
    public virtual string UserName { get; protected set; } = string.Empty;
    public virtual UserRole Role { get; protected set; }
    public virtual string Password { get; protected set; } = string.Empty;

    protected User() { }

    protected User(string userName, UserRole role, string password)
    {
        UserName = userName;
        Role = role;
        Password = password;
    }
}
