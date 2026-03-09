namespace Infrastructure.Tests;

/// <summary>Integration tests for EfProductRepository using EF Core InMemory provider.</summary>
public class EfProductRepositoryTests : IDisposable
{
    private readonly ShoppingDbContext _context;
    private readonly EfProductRepository _repository;

    public EfProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ShoppingDbContext(options);
        _repository = new EfProductRepository(_context);

        // Seed products to match InMemory repo behavior
        _context.Products.AddRange(
            new Product { Name = "Laptop", Description = "High-performance laptop", Price = 15000m, Stock = 10, Category = "Electronics" },
            new Product { Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard", Price = 1500m, Stock = 25, Category = "Electronics" },
            new Product { Name = "Office Chair", Description = "Ergonomic office chair", Price = 4500m, Stock = 15, Category = "Furniture" },
            new Product { Name = "Python Programming Book", Description = "Learn Python the hard way", Price = 600m, Stock = 50, Category = "Books" },
            new Product { Name = "Coffee Mug", Description = "Developer-themed coffee mug", Price = 150m, Stock = 100, Category = "Accessories" }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Add

    [Fact]
    public void Add_NewProduct_ProductIsRetrievableById()
    {
        var product = new Product { Name = "Headphones", Price = 800m, Stock = 15, Category = "Electronics" };

        _repository.Add(product);
        var result = _repository.GetById(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Headphones", result.Name);
    }

    [Fact]
    public void Add_NewProduct_IncreasesTotalCount()
    {
        var countBefore = _repository.GetAll().Count();

        _repository.Add(new Product { Name = "Webcam", Price = 500m, Stock = 10 });

        Assert.Equal(countBefore + 1, _repository.GetAll().Count());
    }

    #endregion

    #region GetById

    [Fact]
    public void GetById_ExistingId_ReturnsCorrectProduct()
    {
        var added = new Product { Name = "Monitor", Price = 4000m, Stock = 5, Category = "Electronics" };
        _repository.Add(added);

        var result = _repository.GetById(added.Id);

        Assert.NotNull(result);
        Assert.Equal("Monitor", result.Name);
    }

    [Fact]
    public void GetById_NonExistentId_ReturnsNull()
    {
        var result = _repository.GetById(99999);

        Assert.Null(result);
    }

    #endregion

    #region GetAll

    [Fact]
    public void GetAll_ReturnsAllSeededProducts()
    {
        var products = _repository.GetAll().ToList();

        Assert.Equal(5, products.Count);
    }

    #endregion

    #region SearchByName

    [Fact]
    public void SearchByName_ExactMatch_ReturnsProduct()
    {
        var results = _repository.SearchByName("Laptop").ToList();

        Assert.Single(results);
        Assert.Equal("Laptop", results[0].Name);
    }

    [Fact]
    public void SearchByName_CaseInsensitive_ReturnsProduct()
    {
        var results = _repository.SearchByName("LAPTOP").ToList();

        Assert.Single(results);
    }

    [Fact]
    public void SearchByName_NoMatch_ReturnsEmpty()
    {
        var results = _repository.SearchByName("ZZZNonExistent").ToList();

        Assert.Empty(results);
    }

    #endregion

    #region SearchByCategory

    [Fact]
    public void SearchByCategory_MatchingCategory_ReturnsAllProductsInCategory()
    {
        var results = _repository.SearchByCategory("Electronics").ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void SearchByCategory_CaseInsensitive_ReturnsProducts()
    {
        var results = _repository.SearchByCategory("electronics").ToList();

        Assert.Equal(2, results.Count);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ExistingProduct_UpdatesAllFields()
    {
        var product = new Product { Name = "Tablet", Price = 5000m, Stock = 10, Category = "Electronics", Description = "Original" };
        _repository.Add(product);

        var updated = new Product
        {
            Id = product.Id,
            Name = "Tablet Pro",
            Price = 8000m,
            Stock = 3,
            Category = "Premium",
            Description = "Updated"
        };
        _repository.Update(updated);

        var result = _repository.GetById(product.Id)!;
        Assert.Equal("Tablet Pro", result.Name);
        Assert.Equal(8000m, result.Price);
        Assert.Equal(3, result.Stock);
        Assert.Equal("Premium", result.Category);
        Assert.Equal("Updated", result.Description);
    }

    [Fact]
    public void Update_NonExistentProduct_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _repository.Update(new Product { Id = 99999, Name = "Ghost", Price = 1m, Stock = 0 }));
    }

    #endregion

    #region Delete

    [Fact]
    public void Delete_ExistingProduct_RemovesItFromCatalog()
    {
        var product = new Product { Name = "OldItem", Price = 10m, Stock = 1 };
        _repository.Add(product);

        _repository.Delete(product.Id);

        Assert.Null(_repository.GetById(product.Id));
    }

    [Fact]
    public void Delete_NonExistentProduct_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => _repository.Delete(99999));
    }

    #endregion

    #region Exists

    [Fact]
    public void Exists_ExistingProduct_ReturnsTrue()
    {
        var product = new Product { Name = "NewProd", Price = 50m, Stock = 5 };
        _repository.Add(product);

        Assert.True(_repository.Exists(product.Id));
    }

    [Fact]
    public void Exists_NonExistentProduct_ReturnsFalse()
    {
        Assert.False(_repository.Exists(99999));
    }

    #endregion
}
