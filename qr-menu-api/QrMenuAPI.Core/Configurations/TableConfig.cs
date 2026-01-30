using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class TableConfig : IEntityTypeConfiguration<TableEntity>
{
    public void Configure(EntityTypeBuilder<TableEntity> builder)
    {
        builder.ToTable("tables");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.EstablishmentId)
            .IsRequired();

        builder.HasOne(t => t.Establishment)
            .WithMany(e => e.Tables)
            .HasForeignKey(t => t.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.EstablishmentId, t.Number })
            .IsUnique();
    }
}
