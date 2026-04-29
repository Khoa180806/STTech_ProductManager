using Abp.Zero.EntityFrameworkCore;
using ProductManager.Authorization.Roles;
using ProductManager.Authorization.Users;
using ProductManager.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using ProductManager.Entities;

namespace ProductManager.EntityFrameworkCore;

public class ProductManagerDbContext : AbpZeroDbContext<Tenant, Role, User, ProductManagerDbContext>
{
    /* Define a DbSet for each entity of the application */
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    
    public ProductManagerDbContext(DbContextOptions<ProductManagerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.Property(p => p.Name).IsRequired().HasMaxLength(256);
            b.Property(p => p.Price).HasColumnType("decimal(18, 2)");
        });
        
        modelBuilder.Entity<Category>(b =>
        {
            b.Property(c => c.Name).IsRequired().HasMaxLength(128);
        });
        base.OnModelCreating(modelBuilder);
    }
}
