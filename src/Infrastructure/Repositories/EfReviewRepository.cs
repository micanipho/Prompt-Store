namespace Infrastructure.Repositories;

/// <summary>EF Core implementation of IReviewRepository backed by SQL Server.</summary>
public class EfReviewRepository : IReviewRepository
{
    private readonly ShoppingDbContext _context;

    public EfReviewRepository(ShoppingDbContext context)
    {
        _context = context;
    }

    public void Add(Review review)
    {
        _context.Reviews.Add(review);
        _context.SaveChanges();
    }

    public IEnumerable<Review> GetByProductId(int productId)
    {
        return _context.Reviews
            .Where(r => r.ProductId == productId)
            .ToList();
    }

    public bool HasCustomerReviewed(int customerId, int productId)
    {
        return _context.Reviews
            .Any(r => r.CustomerId == customerId && r.ProductId == productId);
    }
}
