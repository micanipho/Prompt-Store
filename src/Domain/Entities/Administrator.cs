namespace Domain.Entities;

/// <summary>Represents an administrator who manages products, inventory, and orders.</summary>
public class Administrator : User
{
    protected Administrator() { }

    public Administrator(string userName, string password)
        : base(userName, UserRole.Admin, password) { }
}
