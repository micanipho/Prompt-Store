namespace Domain.Entities;

/// <summary>Represents a product available for purchase in the catalog.</summary>
public class Product
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual decimal Price { get; set; }
    public virtual int Stock { get; set; }
    public virtual string Category { get; set; } = string.Empty;
}
