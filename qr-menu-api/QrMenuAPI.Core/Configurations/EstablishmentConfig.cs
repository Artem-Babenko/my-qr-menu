using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class EstablishmentConfig : IEntityTypeConfiguration<EstablishmentEntity>
{
    public void Configure(EntityTypeBuilder<EstablishmentEntity> builder)
    {
        builder.ToTable("establishments");

        builder.HasKey(e => e.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasMany(e => e.UserEstablishment)
            .WithOne(ue => ue.Establishment)
            .HasForeignKey(ue => ue.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
