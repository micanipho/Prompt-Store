namespace Infrastructure.Repositories;

/// <summary>In-memory implementation of IProductRepository. Stores products in a List for the lifetime of the application.</summary>
public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public InMemoryProductRepository()
    {
        _products.AddRange(new[]
        {
            new Product { Id = _nextId++, Name = "Laptop",                  Description = "High-performance laptop",    Price = 15000m, Stock = 10,  Category = "Electronics" },
            new Product { Id = _nextId++, Name = "Mechanical Keyboard",     Description = "RGB mechanical keyboard",    Price = 1200m,  Stock = 25,  Category = "Electronics" },
            new Product { Id = _nextId++, Name = "Desk Chair",              Description = "Ergonomic office chair",     Price = 3500m,  Stock = 8,   Category = "Furniture"   },
            new Product { Id = _nextId++, Name = "Python Programming Book", Description = "Learn Python from scratch",  Price = 450m,   Stock = 50,  Category = "Books"       },
            new Product { Id = _nextId++, Name = "Coffee Mug",              Description = "Ceramic coffee mug 350ml",  Price = 120m,   Stock = 100, Category = "Kitchen"     },
        });
    }

    /// <summary>Adds a new product, assigning it the next available ID.</summary>
    public void Add(Product product)
    {
        product.Id = _nextId++;
        _products.Add(product);
    }

    /// <summary>Returns the product with the given ID, or null if not found.</summary>
    public Product? GetById(int id) =>
        _products.FirstOrDefault(p => p.Id == id);

    /// <summary>Returns all products in the catalog.</summary>
    public IEnumerable<Product> GetAll() => _products.ToList();

    /// <summary>Returns products whose name contains the search term (case-insensitive).</summary>
    public IEnumerable<Product> SearchByName(string name) =>
        _products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

    /// <summary>Returns products belonging to the given category (case-insensitive exact match).</summary>
    public IEnumerable<Product> SearchByCategory(string category) =>
        _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

    /// <summary>Updates a product's fields in-place. Throws if the product does not exist.</summary>
    public void Update(Product product)
    {
        var existing = _products.FirstOrDefault(p => p.Id == product.Id)
            ?? throw new InvalidOperationException($"Product with ID {product.Id} not found.");

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.Category = product.Category;
    }

    /// <summary>Removes the product with the given ID. Throws if the product does not exist.</summary>
    public void Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id)
            ?? throw new InvalidOperationException($"Product with ID {id} not found.");
        _products.Remove(product);
    }

    /// <summary>Returns true if a product with the given ID exists.</summary>
    public bool Exists(int id) => _products.Any(p => p.Id == id);
}
