
public class Customer : User
{
    public decimal Balance { get; private set; }
    public List<Order> OrderHistory { get; private set; }
    public Cart Cart { get; private set; }

    public Customer(string userName,string Password)
        : base(userName, UserRole.Customer, Password)
    {
        this.Balance = 0;
        this.OrderHistory = new List<Order>();
        this.Cart = new Cart();
    }
}