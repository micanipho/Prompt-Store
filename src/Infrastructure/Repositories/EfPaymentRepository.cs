namespace Infrastructure.Repositories;

/// <summary>EF Core implementation of IPaymentRepository backed by SQL Server.</summary>
public class EfPaymentRepository : IPaymentRepository
{
    private readonly ShoppingDbContext _context;

    public EfPaymentRepository(ShoppingDbContext context)
    {
        _context = context;
    }

    public void Add(Payment payment)
    {
        _context.Payments.Add(payment);
        _context.SaveChanges();
    }

    public Payment? GetByOrderId(int orderId)
    {
        return _context.Payments.FirstOrDefault(p => p.OrderId == orderId);
    }

    public IEnumerable<Payment> GetAll()
    {
        return _context.Payments.ToList();
    }
}
