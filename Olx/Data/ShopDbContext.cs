using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Olx.Models;

namespace Olx.Data;

public class ShopDbContext : IdentityDbContext<User>
{
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<FilterDeclaration> FilterDeclarations { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<FilterValue> FilterValues { get; set; }
    
    public DbSet<Message> Messages { get; set; }
    
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.PhotoUrls).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
        );

        modelBuilder.Entity<FilterValue>().HasOne<FilterDeclaration>(pv => pv.FilterDeclaration)
            .WithMany(pd => pd.FilterValues)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Message>().HasOne<User>(m => m.Sender)
            .WithMany(u => u.Messages)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>().HasOne<Product>(m => m.Product)
            .WithMany(p => p.Messages)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Category>().HasIndex(c => c.NormalizedName).IsUnique();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasMany<Product>(u => u.Favorites)
            .WithMany(p => p.FavoredBy)
            .UsingEntity<Dictionary<string, object>>(
                "Favorites",
                j => j.HasOne<Product>().WithMany().OnDelete(DeleteBehavior.NoAction),
                j => j.HasOne<User>().WithMany().OnDelete(DeleteBehavior.NoAction));

        modelBuilder.Entity<Product>().HasOne<User>(p => p.Owner)
            .WithMany(u => u.Products);
    }
}