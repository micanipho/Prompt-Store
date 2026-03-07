
public class Customer : User
{
    private decimal Balance { get; set; }
    private List<Order> OrderHistory { get; set; }
    private Cart cart { get; set; }

    public Customer(int Id, string userName, UserRole Role, string Password, string Address, string PhoneNumber)
        : base(userName, UserRole.Customer, Password)
    {
        this.Address = Address;
        this.PhoneNumber = PhoneNumber;
    }
}