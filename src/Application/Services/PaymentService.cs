namespace Application.Services;

/// <summary>Handles wallet operations and payment processing for customers.</summary>
public class PaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Returns the current wallet balance for a customer.</summary>
    public static decimal GetBalance(Customer customer) => customer.Balance;

    /// <summary>Adds funds to a customer's wallet balance.</summary>
    public void AddFunds(Customer customer, AddFundsRequest request)
    {
        Guard.Against.NegativeOrZero(request.Amount, message: "Amount must be greater than zero.");
        customer.AddFunds(request.Amount);
        _unitOfWork.SaveChanges();
    }

    /// <summary>
    /// Processes a payment for an order by recording a Payment transaction.
    /// Called after the order has been validated and funds deducted.
    /// </summary>
    public Payment RecordPayment(int orderId, decimal amount)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            PaidAt = DateTime.Now
        };

        _paymentRepository.Add(payment);
        return payment;
    }

    /// <summary>Returns the payment associated with a specific order.</summary>
    public Payment? GetPaymentByOrderId(int orderId) =>
        _paymentRepository.GetByOrderId(orderId);

    /// <summary>Returns all payment records in the system.</summary>
    public IEnumerable<Payment> GetAllPayments() =>
        _paymentRepository.GetAll();
}
