using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class ProductPriceConfig : IEntityTypeConfiguration<ProductPriceEntity>
{
    public void Configure(EntityTypeBuilder<ProductPriceEntity> builder)
    {
        builder.ToTable("product_prices");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pp => pp.Price)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(pp => pp.IsActive)
            .IsRequired();

        builder.HasOne(pp => pp.Product)
            .WithMany()
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pp => pp.Establishment)
            .WithMany()
            .HasForeignKey(pp => pp.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pp => new { pp.ProductId, pp.EstablishmentId })
            .IsUnique();
    }
}
