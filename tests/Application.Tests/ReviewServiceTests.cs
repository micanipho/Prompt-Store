namespace Application.Tests;

/// <summary>Unit tests for ReviewService covering review submission, validation, and retrieval.</summary>
public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        _reviewRepoMock = new Mock<IReviewRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _reviewService = new ReviewService(_reviewRepoMock.Object, _productRepoMock.Object);
    }

    private static Customer CreateCustomerWithOrder(int productId)
    {
        var customer = new Customer("testuser", "password123");
        var product = new Product { Id = productId, Name = "Test Product", Price = 100, Stock = 10 };
        var order = new Order
        {
            Id = 1,
            Items = new List<OrderItem>
            {
                new OrderItem { Product = product, Quantity = 1, UnitPrice = 100 }
            },
            Total = 100,
            Status = OrderStatus.Delivered
        };
        customer.OrderHistory.Add(order);
        return customer;
    }

    #region SubmitReview

    [Fact]
    public void SubmitReview_ValidRequest_AddsReview()
    {
        var customer = CreateCustomerWithOrder(1);
        var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
        _productRepoMock.Setup(r => r.GetById(1)).Returns(product);
        _reviewRepoMock.Setup(r => r.HasCustomerReviewed(customer.Id, 1)).Returns(false);

        var request = new SubmitReviewRequest { ProductId = 1, Rating = 4, Comment = "Great product!" };
        var review = _reviewService.SubmitReview(customer, request);

        Assert.Equal(1, review.ProductId);
        Assert.Equal(customer.Id, review.CustomerId);
        Assert.Equal(4, review.Rating);
        Assert.Equal("Great product!", review.Comment);
        _reviewRepoMock.Verify(r => r.Add(It.IsAny<Review>()), Times.Once);
    }

    [Fact]
    public void SubmitReview_ProductNotPurchased_ThrowsInvalidOperationException()
    {
        var customer = new Customer("testuser", "password123");
        var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
        _productRepoMock.Setup(r => r.GetById(1)).Returns(product);

        var request = new SubmitReviewRequest { ProductId = 1, Rating = 5, Comment = "Nice!" };

        var ex = Assert.Throws<InvalidOperationException>(() => _reviewService.SubmitReview(customer, request));
        Assert.Equal("You can only review products you have purchased.", ex.Message);
    }

    [Fact]
    public void SubmitReview_AlreadyReviewed_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomerWithOrder(1);
        var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
        _productRepoMock.Setup(r => r.GetById(1)).Returns(product);
        _reviewRepoMock.Setup(r => r.HasCustomerReviewed(customer.Id, 1)).Returns(true);

        var request = new SubmitReviewRequest { ProductId = 1, Rating = 5, Comment = "Nice!" };

        var ex = Assert.Throws<InvalidOperationException>(() => _reviewService.SubmitReview(customer, request));
        Assert.Equal("You have already reviewed this product.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void SubmitReview_InvalidRating_ThrowsArgumentOutOfRangeException(int rating)
    {
        var customer = CreateCustomerWithOrder(1);
        var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
        _productRepoMock.Setup(r => r.GetById(1)).Returns(product);
        _reviewRepoMock.Setup(r => r.HasCustomerReviewed(customer.Id, 1)).Returns(false);

        var request = new SubmitReviewRequest { ProductId = 1, Rating = rating, Comment = "Some comment" };

        Assert.Throws<ArgumentOutOfRangeException>(() => _reviewService.SubmitReview(customer, request));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void SubmitReview_EmptyComment_ThrowsArgumentException(string? comment)
    {
        var customer = CreateCustomerWithOrder(1);
        var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
        _productRepoMock.Setup(r => r.GetById(1)).Returns(product);
        _reviewRepoMock.Setup(r => r.HasCustomerReviewed(customer.Id, 1)).Returns(false);

        var request = new SubmitReviewRequest { ProductId = 1, Rating = 4, Comment = comment! };

        Assert.ThrowsAny<ArgumentException>(() => _reviewService.SubmitReview(customer, request));
    }

    [Fact]
    public void SubmitReview_ProductNotFound_ThrowsArgumentNullException()
    {
        var customer = new Customer("testuser", "password123");
        _productRepoMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        var request = new SubmitReviewRequest { ProductId = 99, Rating = 5, Comment = "Nice!" };

        Assert.Throws<ArgumentNullException>(() => _reviewService.SubmitReview(customer, request));
    }

    [Fact]
    public void SubmitReview_TrimsComment()
    {
        var customer = CreateCustomerWithOrder(1);
        var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
        _productRepoMock.Setup(r => r.GetById(1)).Returns(product);
        _reviewRepoMock.Setup(r => r.HasCustomerReviewed(customer.Id, 1)).Returns(false);

        var request = new SubmitReviewRequest { ProductId = 1, Rating = 5, Comment = "  Great!  " };
        var review = _reviewService.SubmitReview(customer, request);

        Assert.Equal("Great!", review.Comment);
    }

    #endregion

    #region GetProductReviews

    [Fact]
    public void GetProductReviews_ReturnsReviewsForProduct()
    {
        var reviews = new List<Review>
        {
            new Review { Id = 1, ProductId = 1, CustomerId = 1, Rating = 5, Comment = "Excellent!" },
            new Review { Id = 2, ProductId = 1, CustomerId = 2, Rating = 3, Comment = "Average." }
        };
        _reviewRepoMock.Setup(r => r.GetByProductId(1)).Returns(reviews);

        var result = _reviewService.GetProductReviews(1).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetProductReviews_NoReviews_ReturnsEmptyList()
    {
        _reviewRepoMock.Setup(r => r.GetByProductId(1)).Returns(new List<Review>());

        var result = _reviewService.GetProductReviews(1).ToList();

        Assert.Empty(result);
    }

    #endregion

    #region GetAverageRating

    [Fact]
    public void GetAverageRating_ReturnsCorrectAverage()
    {
        var reviews = new List<Review>
        {
            new Review { Id = 1, ProductId = 1, Rating = 5 },
            new Review { Id = 2, ProductId = 1, Rating = 3 },
            new Review { Id = 3, ProductId = 1, Rating = 4 }
        };
        _reviewRepoMock.Setup(r => r.GetByProductId(1)).Returns(reviews);

        var result = _reviewService.GetAverageRating(1);

        Assert.Equal(4.0, result);
    }

    [Fact]
    public void GetAverageRating_NoReviews_ReturnsZero()
    {
        _reviewRepoMock.Setup(r => r.GetByProductId(1)).Returns(new List<Review>());

        var result = _reviewService.GetAverageRating(1);

        Assert.Equal(0, result);
    }

    #endregion
}
