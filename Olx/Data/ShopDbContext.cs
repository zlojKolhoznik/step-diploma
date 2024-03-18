using Microsoft.EntityFrameworkCore;
using Olx.Models;

namespace Olx.Data;

public class ShopDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<PropertyDeclaration> PropertyDeclarations { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<PropertyValue> PropertyValues { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.PhotoUrls).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
        );

        modelBuilder.Entity<PropertyValue>().HasOne<PropertyDeclaration>(pv => pv.PropertyDeclaration)
            .WithMany(pd => pd.PropertyValues)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Category>().HasIndex(c => c.NormalizedName).IsUnique();
    }
}