using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>EF Core implementation of IOrderRepository backed by SQL Server.</summary>
public class EfOrderRepository : IOrderRepository
{
    private readonly ShoppingDbContext _context;

    public EfOrderRepository(ShoppingDbContext context)
    {
        _context = context;
    }

    public void Add(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public Order? GetById(int id)
    {
        return _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Order> GetAll()
    {
        return _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Customer)
            .ToList();
    }
}
