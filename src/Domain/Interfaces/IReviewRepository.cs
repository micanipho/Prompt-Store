namespace Domain.Interfaces;

/// <summary>Defines the contract for review persistence operations.</summary>
public interface IReviewRepository
{
    /// <summary>Adds a new review and assigns it a unique ID.</summary>
    void Add(Review review);

    /// <summary>Returns all reviews for a given product.</summary>
    IEnumerable<Review> GetByProductId(int productId);

    /// <summary>Returns true if the customer has already reviewed the given product.</summary>
    bool HasCustomerReviewed(int customerId, int productId);
}
