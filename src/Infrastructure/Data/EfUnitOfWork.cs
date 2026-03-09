namespace Infrastructure.Data;

/// <summary>EF Core implementation of IUnitOfWork that delegates to ShoppingDbContext.</summary>
public class EfUnitOfWork : IUnitOfWork
{
    private readonly ShoppingDbContext _context;

    public EfUnitOfWork(ShoppingDbContext context)
    {
        _context = context;
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
