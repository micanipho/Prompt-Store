namespace Application.Tests;

/// <summary>Unit tests for ReportService covering sales aggregation, revenue, and analytics queries.</summary>
public class ReportServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly ReportService _reportService;

    public ReportServiceTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _reportService = new ReportService(_orderRepoMock.Object, _paymentRepoMock.Object);
    }

    #region GetTotalOrders

    [Fact]
    public void GetTotalOrders_ReturnsCorrectCount()
    {
        var orders = new List<Order>
        {
            new Order { Id = 1, Total = 100 },
            new Order { Id = 2, Total = 200 },
            new Order { Id = 3, Total = 150 }
        };
        _orderRepoMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _reportService.GetTotalOrders();

        Assert.Equal(3, result);
    }

    [Fact]
    public void GetTotalOrders_NoOrders_ReturnsZero()
    {
        _orderRepoMock.Setup(r => r.GetAll()).Returns(new List<Order>());

        var result = _reportService.GetTotalOrders();

        Assert.Equal(0, result);
    }

    #endregion

    #region GetTotalRevenue

    [Fact]
    public void GetTotalRevenue_ReturnsSumOfPayments()
    {
        var payments = new List<Payment>
        {
            new Payment { Id = 1, Amount = 100.50m },
            new Payment { Id = 2, Amount = 250.00m },
            new Payment { Id = 3, Amount = 75.25m }
        };
        _paymentRepoMock.Setup(r => r.GetAll()).Returns(payments);

        var result = _reportService.GetTotalRevenue();

        Assert.Equal(425.75m, result);
    }

    [Fact]
    public void GetTotalRevenue_NoPayments_ReturnsZero()
    {
        _paymentRepoMock.Setup(r => r.GetAll()).Returns(new List<Payment>());

        var result = _reportService.GetTotalRevenue();

        Assert.Equal(0, result);
    }

    #endregion

    #region GetOrdersByStatus

    [Fact]
    public void GetOrdersByStatus_GroupsCorrectly()
    {
        var orders = new List<Order>
        {
            new Order { Id = 1, Status = OrderStatus.Pending, Total = 100 },
            new Order { Id = 2, Status = OrderStatus.Pending, Total = 200 },
            new Order { Id = 3, Status = OrderStatus.Shipped, Total = 150 },
            new Order { Id = 4, Status = OrderStatus.Delivered, Total = 300 }
        };
        _orderRepoMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _reportService.GetOrdersByStatus().ToList();

        Assert.Equal(3, result.Count);
        var pending = result.First(s => s.Status == OrderStatus.Pending);
        Assert.Equal(2, pending.Count);
        Assert.Equal(300, pending.Total);
    }

    [Fact]
    public void GetOrdersByStatus_OrderedByCountDescending()
    {
        var orders = new List<Order>
        {
            new Order { Id = 1, Status = OrderStatus.Delivered, Total = 100 },
            new Order { Id = 2, Status = OrderStatus.Pending, Total = 200 },
            new Order { Id = 3, Status = OrderStatus.Pending, Total = 150 },
            new Order { Id = 4, Status = OrderStatus.Pending, Total = 50 }
        };
        _orderRepoMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _reportService.GetOrdersByStatus().ToList();

        Assert.Equal(OrderStatus.Pending, result[0].Status);
        Assert.Equal(3, result[0].Count);
    }

    [Fact]
    public void GetOrdersByStatus_NoOrders_ReturnsEmptyList()
    {
        _orderRepoMock.Setup(r => r.GetAll()).Returns(new List<Order>());

        var result = _reportService.GetOrdersByStatus().ToList();

        Assert.Empty(result);
    }

    #endregion

    #region GetTopSellingProducts

    [Fact]
    public void GetTopSellingProducts_RankedByQuantitySold()
    {
        var laptop = new Product { Id = 1, Name = "Laptop", Price = 15000 };
        var mouse = new Product { Id = 2, Name = "Mouse", Price = 200 };
        var orders = new List<Order>
        {
            new Order
            {
                Id = 1,
                Items = new List<OrderItem>
                {
                    new OrderItem { Product = laptop, Quantity = 2, UnitPrice = 15000 },
                    new OrderItem { Product = mouse, Quantity = 5, UnitPrice = 200 }
                },
                Total = 31000
            },
            new Order
            {
                Id = 2,
                Items = new List<OrderItem>
                {
                    new OrderItem { Product = mouse, Quantity = 3, UnitPrice = 200 }
                },
                Total = 600
            }
        };
        _orderRepoMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _reportService.GetTopSellingProducts().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Mouse", result[0].ProductName);
        Assert.Equal(8, result[0].TotalQuantitySold);
        Assert.Equal(1600, result[0].TotalRevenue);
        Assert.Equal("Laptop", result[1].ProductName);
        Assert.Equal(2, result[1].TotalQuantitySold);
    }

    [Fact]
    public void GetTopSellingProducts_RespectsCountLimit()
    {
        var products = Enumerable.Range(1, 10).Select(i =>
            new Product { Id = i, Name = $"Product{i}", Price = i * 100 }).ToList();

        var orders = products.Select(p => new Order
        {
            Id = p.Id,
            Items = new List<OrderItem>
            {
                new OrderItem { Product = p, Quantity = p.Id, UnitPrice = p.Price }
            },
            Total = p.Price * p.Id
        }).ToList();
        _orderRepoMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _reportService.GetTopSellingProducts(3).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetTopSellingProducts_NoOrders_ReturnsEmptyList()
    {
        _orderRepoMock.Setup(r => r.GetAll()).Returns(new List<Order>());

        var result = _reportService.GetTopSellingProducts().ToList();

        Assert.Empty(result);
    }

    #endregion

    #region GetAverageOrderValue

    [Fact]
    public void GetAverageOrderValue_ReturnsCorrectAverage()
    {
        var orders = new List<Order>
        {
            new Order { Id = 1, Total = 100 },
            new Order { Id = 2, Total = 200 },
            new Order { Id = 3, Total = 300 }
        };
        _orderRepoMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _reportService.GetAverageOrderValue();

        Assert.Equal(200, result);
    }

    [Fact]
    public void GetAverageOrderValue_NoOrders_ReturnsZero()
    {
        _orderRepoMock.Setup(r => r.GetAll()).Returns(new List<Order>());

        var result = _reportService.GetAverageOrderValue();

        Assert.Equal(0, result);
    }

    #endregion

    #region GetDailySales

    [Fact]
    public void GetDailySales_GroupsByDateAndOrdersDescending()
    {
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);
        var orders = new List<Order>
        {
            new Order { Id = 1, Total = 100, PlacedAt = today.AddHours(10) },
            new Order { Id = 2, Total = 200, PlacedAt = today.AddHours(14) },
            new Order { Id = 3, Total = 150, PlacedAt = yesterday.AddHours(9) }
        };
        _orderRepoMock.Setup(r => r.GetAll()).Returns(orders);

        var result = _reportService.GetDailySales().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(today, result[0].Date);
        Assert.Equal(2, result[0].OrderCount);
        Assert.Equal(300, result[0].Revenue);
        Assert.Equal(yesterday, result[1].Date);
        Assert.Equal(1, result[1].OrderCount);
        Assert.Equal(150, result[1].Revenue);
    }

    [Fact]
    public void GetDailySales_NoOrders_ReturnsEmptyList()
    {
        _orderRepoMock.Setup(r => r.GetAll()).Returns(new List<Order>());

        var result = _reportService.GetDailySales().ToList();

        Assert.Empty(result);
    }

    #endregion
}
