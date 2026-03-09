using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>EF Core database context for the online shopping system.</summary>
public class ShoppingDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();

    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User hierarchy: Table-per-hierarchy with discriminator
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Password).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(u => u.UserName).IsUnique();

            entity.HasDiscriminator(u => u.Role)
                .HasValue<Customer>(UserRole.Customer)
                .HasValue<Administrator>(UserRole.Admin);
        });

        // Customer-specific mappings
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.Balance).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            entity.HasOne(c => c.Cart)
                .WithOne()
                .HasForeignKey<Cart>("CustomerId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.OrderHistory)
                .WithOne(o => o.Customer)
                .HasForeignKey("CustomerId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Category).HasMaxLength(100);
        });

        // Cart
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("Carts");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedOnAdd();

            entity.HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey("CartId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItem
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("CartItems");
            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.Id).ValueGeneratedOnAdd();

            entity.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id).ValueGeneratedOnAdd();
            entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            entity.Property(o => o.PlacedAt).IsRequired();

            entity.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.Id).ValueGeneratedOnAdd();
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.PaidAt).IsRequired();
        });

        // Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.Property(r => r.Comment).HasMaxLength(1000);
            entity.Property(r => r.CreatedAt).IsRequired();

            entity.HasIndex(r => new { r.CustomerId, r.ProductId }).IsUnique();
        });
    }
}
