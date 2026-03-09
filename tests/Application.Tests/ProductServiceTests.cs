using Domain.Factories;
using Moq;

namespace Application.Tests;

/// <summary>Unit tests for ProductService covering all CRUD operations and input validation.</summary>
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly IProductFactory _productFactory;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _productFactory = new ProductFactory();
        _productService = new ProductService(_repositoryMock.Object, _productFactory);
    }

    #region AddProduct

    [Fact]
    public void AddProduct_ValidRequest_AddsProductToRepository()
    {
        _productService.AddProduct(new CreateProductRequest
        {
            Name = "Laptop",
            Description = "A fast laptop",
            Category = "Electronics",
            Price = 15000m,
            Stock = 10
        });

        _repositoryMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public void AddProduct_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _productService.AddProduct(new CreateProductRequest
            {
                Name = "",
                Price = 100m,
                Stock = 5
            }));
    }

    [Fact]
    public void AddProduct_WhitespaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _productService.AddProduct(new CreateProductRequest
            {
                Name = "   ",
                Price = 100m,
                Stock = 5
            }));
    }

    [Fact]
    public void AddProduct_ZeroPrice_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _productService.AddProduct(new CreateProductRequest
            {
                Name = "Laptop",
                Price = 0m,
                Stock = 5
            }));
    }

    [Fact]
    public void AddProduct_NegativePrice_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _productService.AddProduct(new CreateProductRequest
            {
                Name = "Laptop",
                Price = -1m,
                Stock = 5
            }));
    }

    [Fact]
    public void AddProduct_NegativeStock_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _productService.AddProduct(new CreateProductRequest
            {
                Name = "Laptop",
                Price = 100m,
                Stock = -1
            }));
    }

    [Fact]
    public void AddProduct_ZeroStock_AddsProductToRepository()
    {
        // Zero stock is valid — a product can exist before being restocked.
        _productService.AddProduct(new CreateProductRequest
        {
            Name = "Laptop",
            Description = "A fast laptop",
            Category = "Electronics",
            Price = 100m,
            Stock = 0
        });

        _repositoryMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public void AddProduct_InvalidInput_DoesNotCallRepository()
    {
        try
        {
            _productService.AddProduct(new CreateProductRequest { Name = "", Price = 100m, Stock = 5 });
        }
        catch { }

        _repositoryMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region UpdateProduct

    [Fact]
    public void UpdateProduct_ValidRequest_UpdatesRepository()
    {
        _repositoryMock.Setup(r => r.Exists(1)).Returns(true);

        _productService.UpdateProduct(new UpdateProductRequest
        {
            Id = 1,
            Name = "Updated Laptop",
            Description = "Even faster",
            Category = "Electronics",
            Price = 18000m,
            Stock = 5
        });

        _repositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public void UpdateProduct_NonExistentProduct_ThrowsInvalidOperationException()
    {
        _repositoryMock.Setup(r => r.Exists(99)).Returns(false);

        Assert.Throws<InvalidOperationException>(() =>
            _productService.UpdateProduct(new UpdateProductRequest
            {
                Id = 99,
                Name = "Ghost",
                Price = 100m,
                Stock = 0
            }));
    }

    [Fact]
    public void UpdateProduct_EmptyName_ThrowsArgumentException()
    {
        _repositoryMock.Setup(r => r.Exists(1)).Returns(true);

        Assert.Throws<ArgumentException>(() =>
            _productService.UpdateProduct(new UpdateProductRequest
            {
                Id = 1,
                Name = "",
                Price = 100m,
                Stock = 5
            }));
    }

    [Fact]
    public void UpdateProduct_ZeroPrice_ThrowsArgumentException()
    {
        _repositoryMock.Setup(r => r.Exists(1)).Returns(true);

        Assert.Throws<ArgumentException>(() =>
            _productService.UpdateProduct(new UpdateProductRequest
            {
                Id = 1,
                Name = "Laptop",
                Price = 0m,
                Stock = 5
            }));
    }

    [Fact]
    public void UpdateProduct_NegativeStock_ThrowsArgumentException()
    {
        _repositoryMock.Setup(r => r.Exists(1)).Returns(true);

        Assert.Throws<ArgumentException>(() =>
            _productService.UpdateProduct(new UpdateProductRequest
            {
                Id = 1,
                Name = "Laptop",
                Price = 100m,
                Stock = -1
            }));
    }

    #endregion

    #region DeleteProduct

    [Fact]
    public void DeleteProduct_ExistingProduct_CallsRepositoryDelete()
    {
        _repositoryMock.Setup(r => r.Exists(1)).Returns(true);

        _productService.DeleteProduct(1);

        _repositoryMock.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public void DeleteProduct_NonExistentProduct_ThrowsInvalidOperationException()
    {
        _repositoryMock.Setup(r => r.Exists(99)).Returns(false);

        Assert.Throws<InvalidOperationException>(() => _productService.DeleteProduct(99));
    }

    [Fact]
    public void DeleteProduct_NonExistentProduct_DoesNotCallRepositoryDelete()
    {
        _repositoryMock.Setup(r => r.Exists(99)).Returns(false);

        try { _productService.DeleteProduct(99); } catch { }

        _repositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region GetAllProducts

    [Fact]
    public void GetAllProducts_ReturnsAllFromRepository()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop" },
            new Product { Id = 2, Name = "Mouse" }
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(products);

        var result = _productService.GetAllProducts().ToList();

        Assert.Equal(2, result.Count);
    }

    #endregion

    #region GetProductById

    [Fact]
    public void GetProductById_ExistingProduct_ReturnsProduct()
    {
        var product = new Product { Id = 1, Name = "Laptop" };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        var result = _productService.GetProductById(1);

        Assert.Equal(product, result);
    }

    [Fact]
    public void GetProductById_NonExistentProduct_ThrowsInvalidOperationException()
    {
        _repositoryMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        Assert.Throws<InvalidOperationException>(() => _productService.GetProductById(99));
    }

    #endregion

    #region SearchByName

    [Fact]
    public void SearchByName_DelegatesToRepository()
    {
        var expected = new List<Product> { new Product { Id = 1, Name = "Laptop" } };
        _repositoryMock.Setup(r => r.SearchByName("Laptop")).Returns(expected);

        var result = _productService.SearchByName("Laptop").ToList();

        Assert.Equal(expected, result);
    }

    #endregion

    #region SearchByCategory

    [Fact]
    public void SearchByCategory_DelegatesToRepository()
    {
        var expected = new List<Product> { new Product { Id = 1, Name = "Laptop", Category = "Electronics" } };
        _repositoryMock.Setup(r => r.SearchByCategory("Electronics")).Returns(expected);

        var result = _productService.SearchByCategory("Electronics").ToList();

        Assert.Equal(expected, result);
    }

    #endregion
}
