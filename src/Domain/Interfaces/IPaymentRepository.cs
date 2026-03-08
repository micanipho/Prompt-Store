namespace Domain.Interfaces;

/// <summary>Defines the contract for payment persistence operations.</summary>
public interface IPaymentRepository
{
    /// <summary>Adds a new payment record and assigns it a unique ID.</summary>
    void Add(Payment payment);

    /// <summary>Returns all payments for a given order ID.</summary>
    Payment? GetByOrderId(int orderId);

    /// <summary>Returns all payments in the system.</summary>
    IEnumerable<Payment> GetAll();
}
