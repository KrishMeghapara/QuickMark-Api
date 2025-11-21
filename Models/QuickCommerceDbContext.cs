using Microsoft.EntityFrameworkCore;

namespace QuickCommerceAPI.Models;

public class QuickCommerceDbContext : DbContext
{
    public QuickCommerceDbContext(DbContextOptions<QuickCommerceDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Address>().ToTable("Address");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
        modelBuilder.Entity<Cart>().ToTable("Cart");
        modelBuilder.Entity<ProductReview>().ToTable("ProductReview");

        // Add unique index for Email in User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Performance indexes
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.CategoryID);
        
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.ProductName);
        
        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.UserID);
        
        modelBuilder.Entity<Cart>()
            .HasIndex(c => new { c.UserID, c.ProductID })
            .IsUnique();

        // Configure one-to-one relationship between User and Address
        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithOne(a => a.User)
            .HasForeignKey<Address>(a => a.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // Make AddressID nullable in User
        modelBuilder.Entity<User>()
            .Property(u => u.AddressID)
            .IsRequired(false);

        // Make UserID nullable in Address
        modelBuilder.Entity<Address>()
            .Property(a => a.UserID)
            .IsRequired(false);

        // Fix multiple cascade paths for Order
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Address)
            .WithMany(a => a.Orders)
            .HasForeignKey(o => o.AddressID)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Performance optimizations
            optionsBuilder.EnableSensitiveDataLogging(false);
            optionsBuilder.EnableServiceProviderCaching();
            optionsBuilder.EnableDetailedErrors(false);
        }
        
        base.OnConfiguring(optionsBuilder);
    }

}
