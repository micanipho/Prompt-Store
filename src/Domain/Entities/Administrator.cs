namespace Domain.Entities;

/// <summary>Represents an administrator who manages products, inventory, and orders.</summary>
public class Administrator(string userName, string password) : User(userName, UserRole.Admin, password) { }