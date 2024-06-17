using Ether.ServiceDefaults.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ether.ApiService.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new ProductEntityTypeConfiguration().Configure(modelBuilder.Entity<Product>());
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                FirstName = "System",
                LastName = "",
                isActive = true,
                Price = 1.99M
            }
        );
    }
}

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("TB_PRODUTO");
        builder.HasKey(x => x.Id)
                .HasName("IDT_PRODUTO");
        builder.Property(x => x.Price)
            .HasColumnType("decimal(5,2)")
            .HasColumnName("VLR_PRECO");
    }
}
