namespace Infrastructure.Repositories;

/// <summary>In-memory implementation of IReviewRepository using a list.</summary>
public class InMemoryReviewRepository : IReviewRepository
{
    private readonly List<Review> _reviews = [];
    private int _nextId = 1;

    /// <inheritdoc/>
    public void Add(Review review)
    {
        review.Id = _nextId++;
        _reviews.Add(review);
    }

    /// <inheritdoc/>
    public IEnumerable<Review> GetByProductId(int productId) =>
        _reviews.Where(r => r.ProductId == productId).ToList();

    /// <inheritdoc/>
    public bool HasCustomerReviewed(int customerId, int productId) =>
        _reviews.Any(r => r.CustomerId == customerId && r.ProductId == productId);
}
