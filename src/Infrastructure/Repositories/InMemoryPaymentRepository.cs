namespace Infrastructure.Repositories;

/// <summary>In-memory implementation of IPaymentRepository using a list.</summary>
public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _payments = [];
    private int _nextId = 1;

    /// <inheritdoc/>
    public void Add(Payment payment)
    {
        payment.Id = _nextId++;
        _payments.Add(payment);
    }

    /// <inheritdoc/>
    public Payment? GetByOrderId(int orderId) =>
        _payments.FirstOrDefault(p => p.OrderId == orderId);

    /// <inheritdoc/>
    public IEnumerable<Payment> GetAll() => _payments.ToList();
}
