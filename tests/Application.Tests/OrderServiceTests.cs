using Domain.Factories;
using Moq;

namespace Application.Tests;

/// <summary>Unit tests for OrderService covering order placement, retrieval, and status management.</summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDiscountStrategy> _discountStrategyMock;
    private readonly IOrderFactory _orderFactory;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _discountStrategyMock = new Mock<IDiscountStrategy>();
        _orderFactory = new OrderFactory();
        
        // Default mock behavior for strategy
        _discountStrategyMock.Setup(s => s.CalculateTotal(It.IsAny<decimal>())).Returns<decimal>(d => d);
        _discountStrategyMock.Setup(s => s.Name).Returns("No Discount");

        _orderService = new OrderService(
            _orderRepositoryMock.Object, 
            _productRepositoryMock.Object, 
            _paymentRepositoryMock.Object, 
            _unitOfWorkMock.Object, 
            _orderFactory,
            _discountStrategyMock.Object);
    }

    private static Customer CreateCustomer(decimal balance = 5000m)
    {
        var customer = new Customer("testuser", "password");
        if (balance > 0) customer.AddFunds(balance);
        return customer;
    }

    private static Product CreateProduct(int id = 1, string name = "Laptop", decimal price = 1000m, int stock = 10) =>
        new() { Id = id, Name = name, Price = price, Stock = stock, Category = "Electronics" };

    private static void AddToCart(Customer customer, Product product, int quantity) =>
        customer.Cart.Items.Add(new CartItem { Product = product, Quantity = quantity });

    #region PlaceOrder

    [Fact]
    public void PlaceOrder_ValidCartAndBalance_ReturnsOrder()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 2);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);
        _orderRepositoryMock.Setup(r => r.Add(It.IsAny<Order>())).Callback<Order>(o => o.Id = 1);

        var order = _orderService.PlaceOrder(customer);

        Assert.NotNull(order);
        Assert.Equal(2000m, order.Total);
    }

    [Fact]
    public void PlaceOrder_EmptyCart_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();

        Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(customer));
    }

    [Fact]
    public void PlaceOrder_InsufficientBalance_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer(balance: 100m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 2);

        Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(customer));
    }

    [Fact]
    public void PlaceOrder_InsufficientStock_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer(balance: 9999m);
        var product = CreateProduct(price: 100m, stock: 1);
        AddToCart(customer, product, 5);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(customer));
    }

    [Fact]
    public void PlaceOrder_ProductNoLongerExists_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer(balance: 9999m);
        var product = CreateProduct(price: 100m, stock: 10);
        AddToCart(customer, product, 1);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns((Product?)null);

        Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(customer));
    }

    [Fact]
    public void PlaceOrder_Success_DeductsCustomerBalance()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 2);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _orderService.PlaceOrder(customer);

        Assert.Equal(3000m, customer.Balance);
    }

    [Fact]
    public void PlaceOrder_Success_DeductsProductStock()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 3);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _orderService.PlaceOrder(customer);

        Assert.Equal(7, product.Stock);
        _productRepositoryMock.Verify(r => r.Update(product), Times.Once);
    }

    [Fact]
    public void PlaceOrder_Success_ClearsCart()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 1);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _orderService.PlaceOrder(customer);

        Assert.Empty(customer.Cart.Items);
    }

    [Fact]
    public void PlaceOrder_Success_AddsOrderToCustomerHistory()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 1);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _orderService.PlaceOrder(customer);

        Assert.Single(customer.OrderHistory);
    }

    [Fact]
    public void PlaceOrder_Success_OrderStatusIsPending()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 1);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        var order = _orderService.PlaceOrder(customer);

        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void PlaceOrder_Success_SnapshotsUnitPriceAtTimeOfOrder()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 1);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        var order = _orderService.PlaceOrder(customer);
        product.Price = 9999m; // change price after order

        Assert.Equal(1000m, order.Items[0].UnitPrice);
    }

    [Fact]
    public void PlaceOrder_Success_AddsOrderToRepository()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 1);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);

        _orderService.PlaceOrder(customer);

        _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public void PlaceOrder_Success_RecordsPayment()
    {
        var customer = CreateCustomer(balance: 5000m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 2);
        _productRepositoryMock.Setup(r => r.GetById(1)).Returns(product);
        _orderRepositoryMock.Setup(r => r.Add(It.IsAny<Order>())).Callback<Order>(o => o.Id = 1);

        _orderService.PlaceOrder(customer);

        _paymentRepositoryMock.Verify(r => r.Add(It.Is<Payment>(p => p.OrderId == 1 && p.Amount == 2000m)), Times.Once);
    }

    [Fact]
    public void PlaceOrder_InsufficientBalance_DoesNotClearCart()
    {
        var customer = CreateCustomer(balance: 50m);
        var product = CreateProduct(price: 1000m, stock: 10);
        AddToCart(customer, product, 1);

        try { _orderService.PlaceOrder(customer); } catch { }

        Assert.Single(customer.Cart.Items);
    }

    #endregion

    #region GetOrderHistory

    [Fact]
    public void GetOrderHistory_NoOrders_ReturnsEmpty()
    {
        var customer = CreateCustomer();

        var history = _orderService.GetOrderHistory(customer);

        Assert.Empty(history);
    }

    [Fact]
    public void GetOrderHistory_WithOrders_ReturnsAllCustomerOrders()
    {
        var customer = CreateCustomer();
        customer.OrderHistory.Add(new Order { Id = 1, Status = OrderStatus.Pending });
        customer.OrderHistory.Add(new Order { Id = 2, Status = OrderStatus.Shipped });

        var history = _orderService.GetOrderHistory(customer);

        Assert.Equal(2, history.Count());
    }

    [Fact]
    public void GetOrderHistory_ReturnsOrdersSortedMostRecentFirst()
    {
        var customer = CreateCustomer();
        var older = new Order { Id = 1, PlacedAt = DateTime.Now.AddDays(-2) };
        var newer = new Order { Id = 2, PlacedAt = DateTime.Now };
        customer.OrderHistory.Add(older);
        customer.OrderHistory.Add(newer);

        var history = _orderService.GetOrderHistory(customer).ToList();

        Assert.Equal(2, history[0].Id);
        Assert.Equal(1, history[1].Id);
    }

    #endregion

    #region GetAllOrders

    [Fact]
    public void GetAllOrders_NoOrders_ReturnsEmpty()
    {
        _orderRepositoryMock.Setup(r => r.GetAll()).Returns([]);

        var orders = _orderService.GetAllOrders();

        Assert.Empty(orders);
    }

    [Fact]
    public void GetAllOrders_WithOrders_ReturnsAll()
    {
        var orders = new List<Order>
        {
            new() { Id = 1, PlacedAt = DateTime.Now.AddDays(-1) },
            new() { Id = 2, PlacedAt = DateTime.Now }
        };
        _orderRepositoryMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _orderService.GetAllOrders();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetAllOrders_ReturnsSortedMostRecentFirst()
    {
        var orders = new List<Order>
        {
            new() { Id = 1, PlacedAt = DateTime.Now.AddDays(-5) },
            new() { Id = 2, PlacedAt = DateTime.Now }
        };
        _orderRepositoryMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _orderService.GetAllOrders().ToList();

        Assert.Equal(2, result[0].Id);
    }

    #endregion

    #region GetOrderById

    [Fact]
    public void GetOrderById_ExistingId_ReturnsOrder()
    {
        var order = new Order { Id = 1 };
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);

        var result = _orderService.GetOrderById(1);

        Assert.Equal(order, result);
    }

    [Fact]
    public void GetOrderById_NonExistingId_ThrowsInvalidOperationException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        Assert.Throws<InvalidOperationException>(() => _orderService.GetOrderById(99));
    }

    #endregion

    #region UpdateOrderStatus

    [Fact]
    public void UpdateOrderStatus_ValidOrder_UpdatesStatus()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);

        _orderService.UpdateOrderStatus(new UpdateOrderStatusRequest
        {
            OrderId = 1,
            NewStatus = OrderStatus.Shipped
        });

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void UpdateOrderStatus_NonExistingOrder_ThrowsInvalidOperationException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(99)).Returns((Order?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _orderService.UpdateOrderStatus(new UpdateOrderStatusRequest
            {
                OrderId = 99,
                NewStatus = OrderStatus.Delivered
            }));
    }

    [Fact]
    public void UpdateOrderStatus_AllStatusTransitions_Succeed()
    {
        foreach (var status in Enum.GetValues<OrderStatus>())
        {
            var order = new Order { Id = 1, Status = OrderStatus.Pending };
            _orderRepositoryMock.Setup(r => r.GetById(1)).Returns(order);

            _orderService.UpdateOrderStatus(new UpdateOrderStatusRequest { OrderId = 1, NewStatus = status });

            Assert.Equal(status, order.Status);
        }
    }

    #endregion
}
