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
}