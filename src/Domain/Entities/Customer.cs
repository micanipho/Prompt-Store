namespace Domain.Entities;

/// <summary>Represents a customer who can browse products, place orders, and manage their wallet.</summary>
public class Customer : User
{
    public decimal Balance { get; private set; }
    public List<Order> OrderHistory { get; private set; }
    public Cart Cart { get; private set; }

    public Customer(string userName, string password)
        : base(userName, UserRole.Customer, password)
    {
        Balance = 0;
        OrderHistory = [];
        Cart = new Cart();
    }

    /// <summary>Adds funds to the customer's wallet balance.</summary>
    public void AddFunds(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");
        Balance += amount;
    }

    /// <summary>Deducts funds from the customer's wallet balance.</summary>
    public void DeductFunds(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient balance.");
        Balance -= amount;
    }
}