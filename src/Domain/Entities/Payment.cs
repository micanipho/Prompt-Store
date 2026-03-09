namespace Domain.Entities;

/// <summary>Represents a payment transaction made by a customer for an order.</summary>
public class Payment
{
    public virtual int Id { get; set; }
    public virtual int OrderId { get; set; }
    public virtual decimal Amount { get; set; }
    public virtual DateTime PaidAt { get; set; } = DateTime.Now;
}
