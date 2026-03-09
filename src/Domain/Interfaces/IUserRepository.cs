namespace Domain.Interfaces;

/// <summary>Defines the contract for user persistence operations.</summary>
public interface IUserRepository
{
    /// <summary>Adds a new user to the store.</summary>
    void AddUser(User user);

    /// <summary>Returns the user matching the given username, or null if not found.</summary>
    User? GetUserByUsername(string username);

    /// <summary>Returns true if a user with the given username already exists.</summary>
    bool UsernameExists(string username);
}