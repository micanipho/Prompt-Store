using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data;

/// <summary>Factory used by EF Core CLI tools to create the DbContext at design time for migrations.</summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShoppingDbContext>
{
    public ShoppingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShoppingDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=OnlineShoppingDb;User Id=sa;Password=@123Shesha;TrustServerCertificate=True;");
        return new ShoppingDbContext(optionsBuilder.Options);
    }
}
