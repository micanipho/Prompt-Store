namespace Domain.Entities;

/// <summary>Represents a payment transaction made by a customer for an order.</summary>
public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.Now;
}
