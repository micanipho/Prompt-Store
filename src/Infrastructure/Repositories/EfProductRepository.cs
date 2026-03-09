using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>EF Core implementation of IProductRepository backed by SQL Server.</summary>
public class EfProductRepository : IProductRepository
{
    private readonly ShoppingDbContext _context;

    public EfProductRepository(ShoppingDbContext context)
    {
        _context = context;
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public Product? GetById(int id)
    {
        return _context.Products.Find(id);
    }

    public IEnumerable<Product> GetAll()
    {
        return _context.Products.ToList();
    }

    public IEnumerable<Product> SearchByName(string name)
    {
        return _context.Products
            .Where(p => p.Name.ToLower().Contains(name.ToLower()))
            .ToList();
    }

    public IEnumerable<Product> SearchByCategory(string category)
    {
        return _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .ToList();
    }

    public void Update(Product product)
    {
        var existing = _context.Products.Find(product.Id)
            ?? throw new InvalidOperationException($"Product with ID {product.Id} not found.");

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.Category = product.Category;

        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var product = _context.Products.Find(id)
            ?? throw new InvalidOperationException($"Product with ID {id} not found.");

        _context.Products.Remove(product);
        _context.SaveChanges();
    }

    public bool Exists(int id)
    {
        return _context.Products.Any(p => p.Id == id);
    }
}
