using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class ProductPhotoConfig : IEntityTypeConfiguration<ProductPhotoEntity>
{
    public void Configure(EntityTypeBuilder<ProductPhotoEntity> builder)
    {
        builder.ToTable("product_photos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.SortOrder)
            .IsRequired();

        builder.HasOne(p => p.Product)
            .WithMany(p => p.Photos)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.ProductId, p.SortOrder });
    }
}
