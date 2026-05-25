using LoginMVC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoginMVC.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.Property(p => p.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.HasKey(p => p.Id);


        builder.Property(p => p.PurchasePrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.SalePrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Quantity)
            .IsRequired();
    }
}
