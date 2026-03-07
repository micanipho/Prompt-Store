namespace Infrastructure.Tests;

/// <summary>Unit tests for InMemoryProductRepository covering all storage and query operations.</summary>
public class InMemoryProductRepositoryTests
{
    private readonly InMemoryProductRepository _repository;

    public InMemoryProductRepositoryTests()
    {
        _repository = new InMemoryProductRepository();
    }

    #region Constructor / Seeding

    [Fact]
    public void Constructor_SeedsProductCatalog()
    {
        var products = _repository.GetAll().ToList();

        Assert.NotEmpty(products);
    }

    #endregion

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
    public void Add_AssignsUniqueIncrementingIds()
    {
        var first = new Product { Name = "First", Price = 10m, Stock = 1 };
        var second = new Product { Name = "Second", Price = 20m, Stock = 2 };

        _repository.Add(first);
        _repository.Add(second);

        Assert.NotEqual(first.Id, second.Id);
        Assert.True(second.Id > first.Id);
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

    [Fact]
    public void GetAll_AfterAddingProduct_ReflectsNewTotal()
    {
        _repository.Add(new Product { Name = "SSD", Price = 1200m, Stock = 20 });

        Assert.Equal(6, _repository.GetAll().Count());
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
    public void SearchByName_PartialMatch_ReturnsMatchingProducts()
    {
        var results = _repository.SearchByName("Key").ToList();

        Assert.Single(results);
        Assert.Equal("Mechanical Keyboard", results[0].Name);
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

    [Fact]
    public void SearchByCategory_NoMatch_ReturnsEmpty()
    {
        var results = _repository.SearchByCategory("Toys").ToList();

        Assert.Empty(results);
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
    public void Delete_ExistingProduct_DecreasesTotalCount()
    {
        var product = new Product { Name = "TempItem", Price = 10m, Stock = 1 };
        _repository.Add(product);
        var countBefore = _repository.GetAll().Count();

        _repository.Delete(product.Id);

        Assert.Equal(countBefore - 1, _repository.GetAll().Count());
    }

    [Fact]
    public void Delete_NonExistentProduct_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => _repository.Delete(99999));
    }

    #endregion

    #region Exists

    [Fact]
    public void Exists_SeededProduct_ReturnsTrue()
    {
        // ID 1 is always assigned to the first seeded product
        var result = _repository.Exists(1);

        Assert.True(result);
    }

    [Fact]
    public void Exists_NonExistentProduct_ReturnsFalse()
    {
        var result = _repository.Exists(99999);

        Assert.False(result);
    }

    [Fact]
    public void Exists_AfterAddingProduct_ReturnsTrue()
    {
        var product = new Product { Name = "NewProd", Price = 50m, Stock = 5 };
        _repository.Add(product);

        Assert.True(_repository.Exists(product.Id));
    }

    [Fact]
    public void Exists_AfterDeletingProduct_ReturnsFalse()
    {
        var product = new Product { Name = "ToDelete", Price = 50m, Stock = 5 };
        _repository.Add(product);

        _repository.Delete(product.Id);

        Assert.False(_repository.Exists(product.Id));
    }

    #endregion
}
