using DiscountCode.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

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
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration["DEFAULT_CONNECTION_STRING"];

        optionsBuilder.UseNpgsql(connectionString);

        return new DiscountDbContext(optionsBuilder.Options);
    }
}