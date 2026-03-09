namespace Domain.Factories;

using Domain.Entities;

/// <summary>Factory for creating Product entities.</summary>
public interface IProductFactory
{
    /// <summary>Creates a new Product entity.</summary>
    Product CreateProduct(string name, string description, decimal price, int stock, string category);
}

/// <summary>Implementation of the ProductFactory.</summary>
public class ProductFactory : IProductFactory
{
    public Product CreateProduct(string name, string description, decimal price, int stock, string category)
    {
        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            Category = category
        };
    }
}
