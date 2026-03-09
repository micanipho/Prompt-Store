namespace Application.Tests;

/// <summary>Unit tests for CartService covering add, update, remove, total, and clear operations.</summary>
public class CartServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cartService = new CartService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static Customer CreateCustomer() => new("testuser", "password");

    private static Product CreateProduct(int id = 1, string name = "Laptop", decimal price = 15000m, int stock = 10) =>
        new() { Id = id, Name = name, Price = price, Stock = stock, Category = "Electronics" };

    #region AddToCart

    [Fact]
    public void AddToCart_ValidRequest_AddsItemToCart()
    {
        var customer = CreateCustomer();
        var product = CreateProduct();
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = 2 });

        Assert.Single(customer.Cart.Items);
        Assert.Equal(product, customer.Cart.Items[0].Product);
        Assert.Equal(2, customer.Cart.Items[0].Quantity);
    }

    [Fact]
    public void AddToCart_ProductNotFound_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();
        _repositoryMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 99, Quantity = 1 }));
    }

    [Fact]
    public void AddToCart_InsufficientStock_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();
        var product = CreateProduct(stock: 5);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        Assert.Throws<InvalidOperationException>(() =>
            _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = 10 }));
    }

    [Fact]
    public void AddToCart_ZeroQuantity_ThrowsArgumentException()
    {
        var customer = CreateCustomer();

        Assert.Throws<ArgumentException>(() =>
            _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = 0 }));
    }

    [Fact]
    public void AddToCart_NegativeQuantity_ThrowsArgumentException()
    {
        var customer = CreateCustomer();

        Assert.Throws<ArgumentException>(() =>
            _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = -1 }));
    }

    [Fact]
    public void AddToCart_ExistingProduct_IncrementsQuantity()
    {
        var customer = CreateCustomer();
        var product = CreateProduct(stock: 10);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = 3 });
        _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = 2 });

        Assert.Single(customer.Cart.Items);
        Assert.Equal(5, customer.Cart.Items[0].Quantity);
    }

    [Fact]
    public void AddToCart_ExistingProduct_CombinedQuantityExceedsStock_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();
        var product = CreateProduct(stock: 5);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = 3 });

        Assert.Throws<InvalidOperationException>(() =>
            _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 1, Quantity = 3 }));
    }

    [Fact]
    public void AddToCart_InvalidRequest_DoesNotModifyCart()
    {
        var customer = CreateCustomer();
        _repositoryMock.Setup(r => r.GetById(99)).Returns((Product?)null);

        try { _cartService.AddToCart(customer, new AddToCartRequest { ProductId = 99, Quantity = 1 }); } catch { }

        Assert.Empty(customer.Cart.Items);
    }

    #endregion

    #region UpdateCartItem

    [Fact]
    public void UpdateCartItem_ValidRequest_UpdatesQuantity()
    {
        var customer = CreateCustomer();
        var product = CreateProduct(stock: 10);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);
        customer.Cart.Items.Add(new CartItem { Product = product, Quantity = 3 });

        _cartService.UpdateCartItem(customer, new UpdateCartItemRequest { ProductId = 1, NewQuantity = 5 });

        Assert.Equal(5, customer.Cart.Items[0].Quantity);
    }

    [Fact]
    public void UpdateCartItem_ZeroQuantity_RemovesItem()
    {
        var customer = CreateCustomer();
        var product = CreateProduct();
        customer.Cart.Items.Add(new CartItem { Product = product, Quantity = 3 });

        _cartService.UpdateCartItem(customer, new UpdateCartItemRequest { ProductId = 1, NewQuantity = 0 });

        Assert.Empty(customer.Cart.Items);
    }

    [Fact]
    public void UpdateCartItem_ProductNotInCart_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();

        Assert.Throws<InvalidOperationException>(() =>
            _cartService.UpdateCartItem(customer, new UpdateCartItemRequest { ProductId = 99, NewQuantity = 1 }));
    }

    [Fact]
    public void UpdateCartItem_NegativeQuantity_ThrowsArgumentException()
    {
        var customer = CreateCustomer();

        Assert.Throws<ArgumentException>(() =>
            _cartService.UpdateCartItem(customer, new UpdateCartItemRequest { ProductId = 1, NewQuantity = -1 }));
    }

    [Fact]
    public void UpdateCartItem_InsufficientStock_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();
        var product = CreateProduct(stock: 5);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(product);
        customer.Cart.Items.Add(new CartItem { Product = product, Quantity = 3 });

        Assert.Throws<InvalidOperationException>(() =>
            _cartService.UpdateCartItem(customer, new UpdateCartItemRequest { ProductId = 1, NewQuantity = 10 }));
    }

    #endregion

    #region RemoveFromCart

    [Fact]
    public void RemoveFromCart_ValidProduct_RemovesItem()
    {
        var customer = CreateCustomer();
        var product = CreateProduct();
        customer.Cart.Items.Add(new CartItem { Product = product, Quantity = 2 });

        _cartService.RemoveFromCart(customer, 1);

        Assert.Empty(customer.Cart.Items);
    }

    [Fact]
    public void RemoveFromCart_ProductNotInCart_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();

        Assert.Throws<InvalidOperationException>(() => _cartService.RemoveFromCart(customer, 99));
    }

    #endregion

    #region GetCartTotal

    [Fact]
    public void GetCartTotal_EmptyCart_ReturnsZero()
    {
        var customer = CreateCustomer();

        var total = _cartService.GetCartTotal(customer);

        Assert.Equal(0m, total);
    }

    [Fact]
    public void GetCartTotal_MultipleItems_ReturnsCorrectSum()
    {
        var customer = CreateCustomer();
        var laptop = CreateProduct(id: 1, name: "Laptop", price: 15000m);
        var mouse = CreateProduct(id: 2, name: "Mouse", price: 250m);
        customer.Cart.Items.Add(new CartItem { Product = laptop, Quantity = 1 });
        customer.Cart.Items.Add(new CartItem { Product = mouse, Quantity = 3 });

        var total = _cartService.GetCartTotal(customer);

        Assert.Equal(15750m, total);
    }

    #endregion

    #region ClearCart

    [Fact]
    public void ClearCart_NonEmptyCart_RemovesAllItems()
    {
        var customer = CreateCustomer();
        customer.Cart.Items.Add(new CartItem { Product = CreateProduct(id: 1), Quantity = 1 });
        customer.Cart.Items.Add(new CartItem { Product = CreateProduct(id: 2), Quantity = 2 });

        _cartService.ClearCart(customer);

        Assert.Empty(customer.Cart.Items);
    }

    #endregion
}
