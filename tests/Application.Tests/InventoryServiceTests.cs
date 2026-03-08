namespace Application.Tests;

/// <summary>Unit tests for InventoryService covering restocking, low-stock queries, and validation.</summary>
public class InventoryServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _inventoryService = new InventoryService(_repositoryMock.Object);
    }

    #region RestockProduct

    [Fact]
    public void RestockProduct_ValidRequest_IncreasesStockByQuantity()
    {
        var product = new Product { Id = 1, Name = "Laptop", Stock = 5 };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _inventoryService.RestockProduct(new RestockProductRequest { ProductId = 1, Quantity = 10 });

        Assert.Equal(15, product.Stock);
        _repositoryMock.Verify(r => r.Update(product), Times.Once);
    }

    [Fact]
    public void RestockProduct_ZeroQuantity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _inventoryService.RestockProduct(new RestockProductRequest { ProductId = 1, Quantity = 0 }));
    }

    [Fact]
    public void RestockProduct_NegativeQuantity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _inventoryService.RestockProduct(new RestockProductRequest { ProductId = 1, Quantity = -5 }));
    }

    [Fact]
    public void RestockProduct_NonExistentProduct_ThrowsInvalidOperationException()
    {
        _repositoryMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _inventoryService.RestockProduct(new RestockProductRequest { ProductId = 99, Quantity = 10 }));
    }

    [Fact]
    public void RestockProduct_InvalidQuantity_DoesNotCallRepository()
    {
        try
        {
            _inventoryService.RestockProduct(new RestockProductRequest { ProductId = 1, Quantity = 0 });
        }
        catch { }

        _repositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region GetLowStockProducts

    [Fact]
    public void GetLowStockProducts_ReturnsProductsAtOrBelowThreshold()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Stock = 10 },
            new Product { Id = 2, Name = "Mouse", Stock = 3 },
            new Product { Id = 3, Name = "Keyboard", Stock = 5 },
            new Product { Id = 4, Name = "Monitor", Stock = 0 }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(products);

        var result = _inventoryService.GetLowStockProducts().ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name == "Mouse");
        Assert.Contains(result, p => p.Name == "Keyboard");
        Assert.Contains(result, p => p.Name == "Monitor");
        Assert.DoesNotContain(result, p => p.Name == "Laptop");
    }

    [Fact]
    public void GetLowStockProducts_OrdersByStockAscending()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Keyboard", Stock = 5 },
            new Product { Id = 2, Name = "Monitor", Stock = 0 },
            new Product { Id = 3, Name = "Mouse", Stock = 3 }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(products);

        var result = _inventoryService.GetLowStockProducts().ToList();

        Assert.Equal("Monitor", result[0].Name);
        Assert.Equal("Mouse", result[1].Name);
        Assert.Equal("Keyboard", result[2].Name);
    }

    [Fact]
    public void GetLowStockProducts_NoLowStockProducts_ReturnsEmptyList()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Stock = 50 },
            new Product { Id = 2, Name = "Mouse", Stock = 20 }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(products);

        var result = _inventoryService.GetLowStockProducts().ToList();

        Assert.Empty(result);
    }

    #endregion

    #region GetOutOfStockProducts

    [Fact]
    public void GetOutOfStockProducts_ReturnsOnlyZeroStockProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Stock = 10 },
            new Product { Id = 2, Name = "Mouse", Stock = 0 },
            new Product { Id = 3, Name = "Keyboard", Stock = 0 },
            new Product { Id = 4, Name = "Monitor", Stock = 3 }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(products);

        var result = _inventoryService.GetOutOfStockProducts().ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal(0, p.Stock));
    }

    [Fact]
    public void GetOutOfStockProducts_NoneOutOfStock_ReturnsEmptyList()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Stock = 10 }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(products);

        var result = _inventoryService.GetOutOfStockProducts().ToList();

        Assert.Empty(result);
    }

    #endregion

    #region GetStockLevel

    [Fact]
    public void GetStockLevel_ExistingProduct_ReturnsCurrentStock()
    {
        var product = new Product { Id = 1, Name = "Laptop", Stock = 42 };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        var stock = _inventoryService.GetStockLevel(1);

        Assert.Equal(42, stock);
    }

    [Fact]
    public void GetStockLevel_NonExistentProduct_ThrowsInvalidOperationException()
    {
        _repositoryMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        Assert.Throws<InvalidOperationException>(() => _inventoryService.GetStockLevel(99));
    }

    #endregion
}
