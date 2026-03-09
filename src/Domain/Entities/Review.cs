namespace Domain.Entities;

/// <summary>Represents a customer review and rating for a product.</summary>
public class Review
{
    public virtual int Id { get; set; }
    public virtual int ProductId { get; set; }
    public virtual int CustomerId { get; set; }
    public virtual int Rating { get; set; }
    public virtual string Comment { get; set; } = string.Empty;
    public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
}
