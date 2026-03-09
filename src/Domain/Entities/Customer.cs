namespace Domain.Entities;

/// <summary>Represents a customer who can browse products, place orders, and manage their wallet.</summary>
public class Customer : User
{
    public virtual decimal Balance { get; protected set; }
    public virtual IList<Order> OrderHistory { get; protected set; } = new List<Order>();
    public virtual Cart Cart { get; protected set; } = new Cart();

    protected Customer() { }

    public Customer(string userName, string password)
        : base(userName, UserRole.Customer, password)
    {
        Balance = 0;
        OrderHistory = new List<Order>();
        Cart = new Cart();
    }

    /// <summary>Adds funds to the customer's wallet balance.</summary>
    public virtual void AddFunds(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");
        Balance += amount;
    }

    /// <summary>Deducts funds from the customer's wallet balance.</summary>
    public virtual void DeductFunds(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient balance.");
        Balance -= amount;
    }
}
