using DiscountCode.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscountCode.Infrastructure.Persistance;

public class DiscountDbContext : DbContext
{
    public DiscountDbContext(DbContextOptions<DiscountDbContext> options) : base(options)
    {
    }

    public DbSet<DiscountCodeModel> DiscountCodes { get; set; }
    public DbSet<AvailableDiscountCodeModel> AvailableDiscountCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountCodeModel>()
            .HasIndex(d => d.Code)
            .IsUnique();

        modelBuilder.Entity<AvailableDiscountCodeModel>()
            .HasIndex(u => u.Code)
            .IsUnique();
    }
}

public class DiscountDbContextFactory : IDesignTimeDbContextFactory<DiscountDbContext>
{
    public DiscountDbContext CreateDbContext(string[] args = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DiscountDbContext>();

        // Use your connection string here
        optionsBuilder.UseNpgsql("Host=db;Database=discountdb;Username=postgres;Password=YourStrong@Passw0rd");

        return new DiscountDbContext(optionsBuilder.Options);
    }
}