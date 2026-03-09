namespace Application.Tests;

/// <summary>Unit tests for PaymentService covering wallet operations and payment recording.</summary>
public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _paymentService = new PaymentService(_paymentRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static Customer CreateCustomer(decimal balance = 0m)
    {
        var customer = new Customer("testuser", "password");
        if (balance > 0) customer.AddFunds(balance);
        return customer;
    }

    #region GetBalance

    [Fact]
    public void GetBalance_NewCustomer_ReturnsZero()
    {
        var customer = CreateCustomer();

        var balance = PaymentService.GetBalance(customer);

        Assert.Equal(0m, balance);
    }

    [Fact]
    public void GetBalance_CustomerWithFunds_ReturnsCorrectBalance()
    {
        var customer = CreateCustomer(balance: 500m);

        var balance = PaymentService.GetBalance(customer);

        Assert.Equal(500m, balance);
    }

    #endregion

    #region AddFunds

    [Fact]
    public void AddFunds_ValidAmount_IncreasesBalance()
    {
        var customer = CreateCustomer();

        _paymentService.AddFunds(customer, new AddFundsRequest { Amount = 100m });

        Assert.Equal(100m, customer.Balance);
    }

    [Fact]
    public void AddFunds_MultipleTimes_AccumulatesBalance()
    {
        var customer = CreateCustomer();

        _paymentService.AddFunds(customer, new AddFundsRequest { Amount = 100m });
        _paymentService.AddFunds(customer, new AddFundsRequest { Amount = 250m });

        Assert.Equal(350m, customer.Balance);
    }

    [Fact]
    public void AddFunds_ZeroAmount_ThrowsException()
    {
        var customer = CreateCustomer();

        Assert.ThrowsAny<Exception>(() =>
            _paymentService.AddFunds(customer, new AddFundsRequest { Amount = 0m }));
    }

    [Fact]
    public void AddFunds_NegativeAmount_ThrowsException()
    {
        var customer = CreateCustomer();

        Assert.ThrowsAny<Exception>(() =>
            _paymentService.AddFunds(customer, new AddFundsRequest { Amount = -50m }));
    }

    #endregion

    #region RecordPayment

    [Fact]
    public void RecordPayment_ValidInput_ReturnsPaymentWithCorrectDetails()
    {
        var payment = _paymentService.RecordPayment(orderId: 1, amount: 500m);

        Assert.Equal(1, payment.OrderId);
        Assert.Equal(500m, payment.Amount);
    }

    [Fact]
    public void RecordPayment_ValidInput_AddsToRepository()
    {
        _paymentService.RecordPayment(orderId: 1, amount: 500m);

        _paymentRepositoryMock.Verify(r => r.Add(It.Is<Payment>(p =>
            p.OrderId == 1 && p.Amount == 500m)), Times.Once);
    }

    #endregion

    #region GetPaymentByOrderId

    [Fact]
    public void GetPaymentByOrderId_ExistingPayment_ReturnsPayment()
    {
        var expected = new Payment { Id = 1, OrderId = 5, Amount = 200m };
        _paymentRepositoryMock.Setup(r => r.GetByOrderId(5)).Returns(expected);

        var result = _paymentService.GetPaymentByOrderId(5);

        Assert.NotNull(result);
        Assert.Equal(5, result.OrderId);
    }

    [Fact]
    public void GetPaymentByOrderId_NonExistingPayment_ReturnsNull()
    {
        _paymentRepositoryMock.Setup(r => r.GetByOrderId(99)).Returns((Payment?)null);

        var result = _paymentService.GetPaymentByOrderId(99);

        Assert.Null(result);
    }

    #endregion

    #region GetAllPayments

    [Fact]
    public void GetAllPayments_NoPayments_ReturnsEmpty()
    {
        _paymentRepositoryMock.Setup(r => r.GetAll()).Returns([]);

        var result = _paymentService.GetAllPayments();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAllPayments_WithPayments_ReturnsAll()
    {
        var payments = new List<Payment>
        {
            new() { Id = 1, OrderId = 1, Amount = 100m },
            new() { Id = 2, OrderId = 2, Amount = 200m }
        };
        _paymentRepositoryMock.Setup(r => r.GetAll()).Returns(payments);

        var result = _paymentService.GetAllPayments();

        Assert.Equal(2, result.Count());
    }

    #endregion
}
