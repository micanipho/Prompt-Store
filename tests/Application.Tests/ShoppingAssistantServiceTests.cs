using Application.Interfaces;

namespace Application.Tests;

/// <summary>Unit tests for ShoppingAssistantService covering chat orchestration, context building, and edge cases.</summary>
public class ShoppingAssistantServiceTests
{
    private readonly Mock<IShoppingAssistant> _assistantMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly ShoppingAssistantService _service;
    private readonly Customer _customer;

    public ShoppingAssistantServiceTests()
    {
        _assistantMock = new Mock<IShoppingAssistant>();
        _productRepoMock = new Mock<IProductRepository>();
        _orderRepoMock = new Mock<IOrderRepository>();

        var productService = new ProductService(_productRepoMock.Object, new Domain.Factories.ProductFactory());
        var cartService = new CartService(_productRepoMock.Object, Mock.Of<IUnitOfWork>());
        var orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            Mock.Of<IPaymentRepository>(),
            Mock.Of<IUnitOfWork>(),
            new Domain.Factories.OrderFactory());

        _service = new ShoppingAssistantService(_assistantMock.Object, productService, cartService, orderService);

        _customer = new Customer("testuser", "password");
    }

    #region ChatAsync – basic flow

    [Fact]
    public async Task ChatAsync_ValidMessage_ReturnsAssistantResponse()
    {
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Here are some recommendations.");

        var result = await _service.ChatAsync(_customer, "Show me some products", []);

        Assert.Equal("Here are some recommendations.", result);
    }

    [Fact]
    public async Task ChatAsync_ValidMessage_AppendsBothMessagesToHistory()
    {
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Sure, here you go.");

        var history = new List<ChatMessage>();
        await _service.ChatAsync(_customer, "What do you have?", history);

        Assert.Equal(2, history.Count);
        Assert.Equal("user", history[0].Role);
        Assert.Equal("What do you have?", history[0].Content);
        Assert.Equal("assistant", history[1].Role);
        Assert.Equal("Sure, here you go.", history[1].Content);
    }

    [Fact]
    public async Task ChatAsync_MultiTurn_PassesPriorHistoryToAssistant()
    {
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);

        var capturedHistory = new List<ChatMessage>();
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .Callback<string, ShoppingContext, IList<ChatMessage>, CancellationToken>((_, _, history, _) =>
                capturedHistory.AddRange(history))
            .ReturnsAsync("Response.");

        var history = new List<ChatMessage>();

        // First turn
        await _service.ChatAsync(_customer, "First message", history);

        // Second turn
        capturedHistory.Clear();
        await _service.ChatAsync(_customer, "Second message", history);

        // On the second call the history passed to the assistant should include the first exchange
        Assert.Equal(2, capturedHistory.Count);
        Assert.Equal("user", capturedHistory[0].Role);
        Assert.Equal("First message", capturedHistory[0].Content);
    }

    #endregion

    #region ChatAsync – context building

    [Fact]
    public async Task ChatAsync_BuildsContextWithAllProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Category = "Electronics", Price = 15000m, Stock = 5 },
            new() { Id = 2, Name = "Mouse", Category = "Electronics", Price = 250m, Stock = 10 }
        };
        _productRepoMock.Setup(r => r.GetAll()).Returns(products);

        ShoppingContext? capturedContext = null;
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .Callback<string, ShoppingContext, IList<ChatMessage>, CancellationToken>((_, ctx, _, _) => capturedContext = ctx)
            .ReturnsAsync("OK");

        await _service.ChatAsync(_customer, "Show products", []);

        Assert.NotNull(capturedContext);
        Assert.Equal(2, capturedContext!.Products.Count);
    }

    [Fact]
    public async Task ChatAsync_BuildsContextWithCustomerName()
    {
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);

        ShoppingContext? capturedContext = null;
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .Callback<string, ShoppingContext, IList<ChatMessage>, CancellationToken>((_, ctx, _, _) => capturedContext = ctx)
            .ReturnsAsync("OK");

        await _service.ChatAsync(_customer, "Hello", []);

        Assert.Equal("testuser", capturedContext!.CustomerName);
    }

    [Fact]
    public async Task ChatAsync_BuildsContextWithCartItems()
    {
        var product = new Product { Id = 1, Name = "Laptop", Price = 15000m, Stock = 5 };
        _customer.Cart.Items.Add(new CartItem { Product = product, Quantity = 2 });
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);

        ShoppingContext? capturedContext = null;
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .Callback<string, ShoppingContext, IList<ChatMessage>, CancellationToken>((_, ctx, _, _) => capturedContext = ctx)
            .ReturnsAsync("OK");

        await _service.ChatAsync(_customer, "What's in my cart?", []);

        Assert.Single(capturedContext!.CartItems);
        Assert.Equal("Laptop", capturedContext.CartItems[0].Product.Name);
    }

    [Fact]
    public async Task ChatAsync_BuildsContextWithRecentOrders_LimitedToFive()
    {
        for (var i = 1; i <= 8; i++)
            _customer.OrderHistory.Add(new Order { Id = i, PlacedAt = DateTime.Now.AddDays(-i) });

        _productRepoMock.Setup(r => r.GetAll()).Returns([]);

        ShoppingContext? capturedContext = null;
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .Callback<string, ShoppingContext, IList<ChatMessage>, CancellationToken>((_, ctx, _, _) => capturedContext = ctx)
            .ReturnsAsync("OK");

        await _service.ChatAsync(_customer, "Track my orders", []);

        Assert.Equal(5, capturedContext!.RecentOrders.Count);
    }

    #endregion

    #region ChatAsync – input validation

    [Fact]
    public async Task ChatAsync_EmptyMessage_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ChatAsync(_customer, "", []));
    }

    [Fact]
    public async Task ChatAsync_WhitespaceMessage_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ChatAsync(_customer, "   ", []));
    }

    [Fact]
    public async Task ChatAsync_InvalidInput_DoesNotCallAssistant()
    {
        try { await _service.ChatAsync(_customer, "", []); } catch { }

        _assistantMock.Verify(
            a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion

    #region ChatAsync – provider failure

    [Fact]
    public async Task ChatAsync_AssistantThrows_ExceptionPropagates()
    {
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _service.ChatAsync(_customer, "Hello", []));
    }

    [Fact]
    public async Task ChatAsync_AssistantThrows_HistoryIsNotUpdated()
    {
        _productRepoMock.Setup(r => r.GetAll()).Returns([]);
        _assistantMock
            .Setup(a => a.GetResponseAsync(It.IsAny<string>(), It.IsAny<ShoppingContext>(), It.IsAny<IList<ChatMessage>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        var history = new List<ChatMessage>();
        try { await _service.ChatAsync(_customer, "Hello", history); } catch { }

        Assert.Empty(history);
    }

    #endregion
}
