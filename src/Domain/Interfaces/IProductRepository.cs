namespace Domain.Interfaces;

/// <summary>Defines the contract for product catalog persistence operations.</summary>
public interface IProductRepository
{
    /// <summary>Adds a new product to the store.</summary>
    void Add(Product product);

    /// <summary>Returns the product with the given ID, or null if not found.</summary>
    Product? GetById(int id);

    /// <summary>Returns all products in the catalog.</summary>
    IEnumerable<Product> GetAll();

    /// <summary>Returns products whose name contains the search term (case-insensitive).</summary>
    IEnumerable<Product> SearchByName(string name);

    /// <summary>Returns products belonging to the given category (case-insensitive).</summary>
    IEnumerable<Product> SearchByCategory(string category);

    /// <summary>Updates an existing product's fields in-place.</summary>
    void Update(Product product);

    /// <summary>Removes the product with the given ID.</summary>
    void Delete(int id);

    /// <summary>Returns true if a product with the given ID exists.</summary>
    bool Exists(int id);
}
