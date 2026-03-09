using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>Seeds the database with initial data if it is empty.</summary>
public static class DatabaseSeeder
{
    /// <summary>Ensures the database is created and seeds default admin and product catalog.</summary>
    public static void Seed(ShoppingDbContext context)
    {
        context.Database.Migrate();

        if (!context.Users.Any())
        {
            context.Users.Add(new Administrator("admin", "admin123"));
            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Laptop", Description = "High-performance laptop", Price = 15000m, Stock = 10, Category = "Electronics" },
                new Product { Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard", Price = 1500m, Stock = 25, Category = "Electronics" },
                new Product { Name = "Office Chair", Description = "Ergonomic office chair", Price = 4500m, Stock = 15, Category = "Furniture" },
                new Product { Name = "Python Programming Book", Description = "Learn Python the hard way", Price = 600m, Stock = 50, Category = "Books" },
                new Product { Name = "Coffee Mug", Description = "Developer-themed coffee mug", Price = 150m, Stock = 100, Category = "Accessories" }
            );
            context.SaveChanges();
        }
    }
}
