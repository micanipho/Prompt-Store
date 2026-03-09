namespace Domain.Factories;

using Domain.Entities;
using Domain.Enums;

/// <summary>Factory interface for creating different types of users.</summary>
public interface IUserFactory
{
    /// <summary>Creates a user based on the specified role.</summary>
    User CreateUser(string userName, string password, UserRole role);
}

/// <summary>Concrete implementation of the UserFactory.</summary>
public class UserFactory : IUserFactory
{
    public User CreateUser(string userName, string password, UserRole role)
    {
        return role switch
        {
            UserRole.Customer => new Customer(userName, password),
            UserRole.Admin => new Administrator(userName, password),
            _ => throw new InvalidOperationException($"Invalid user role: {role}")
        };
    }
}
