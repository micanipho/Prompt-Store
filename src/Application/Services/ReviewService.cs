namespace Application.Services;

/// <summary>Handles product review submission and retrieval, ensuring customers can only review purchased products.</summary>
public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;

    public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
    }

    /// <summary>Submits a review for a product the customer has purchased. Validates purchase history, rating range, and duplicate reviews.</summary>
    public Review SubmitReview(Customer customer, SubmitReviewRequest request)
    {
        Guard.Against.Null(customer);
        Guard.Against.Null(request);

        var product = _productRepository.GetById(request.ProductId);
        Guard.Against.Null(product, message: $"Product with ID {request.ProductId} not found.");

        bool hasPurchased = customer.OrderHistory
            .Any(order => order.Items.Any(item => item.Product.Id == request.ProductId));

        if (!hasPurchased)
            throw new InvalidOperationException("You can only review products you have purchased.");

        if (_reviewRepository.HasCustomerReviewed(customer.Id, request.ProductId))
            throw new InvalidOperationException("You have already reviewed this product.");

        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(request.Comment))
            throw new ArgumentException("Comment cannot be empty.");

        var review = new Review
        {
            ProductId = request.ProductId,
            CustomerId = customer.Id,
            Rating = request.Rating,
            Comment = request.Comment.Trim()
        };

        _reviewRepository.Add(review);
        return review;
    }

    /// <summary>Returns all reviews for a given product.</summary>
    public IEnumerable<Review> GetProductReviews(int productId) =>
        _reviewRepository.GetByProductId(productId);

    /// <summary>Returns the average rating for a product, or 0 if no reviews exist.</summary>
    public double GetAverageRating(int productId)
    {
        var reviews = _reviewRepository.GetByProductId(productId).ToList();
        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }
}
