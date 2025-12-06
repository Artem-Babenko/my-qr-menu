using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class NetworkConfig : IEntityTypeConfiguration<NetworkEntity>
{
    public void Configure(EntityTypeBuilder<NetworkEntity> builder)
    {
        builder.ToTable("networks");

        builder.HasKey(n => n.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(n => n.Name)
            .IsUnique();

        builder.Property(n => n.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.HasMany(n => n.Establishments)
            .WithOne(e => e.Network)
            .HasForeignKey(e => e.NetworkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(n => n.Roles)
            .WithOne(r => r.Network)
            .HasForeignKey(r => r.NetworkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(n => n.Users)
            .WithOne(u => u.Network)
            .HasForeignKey(u => u.NetworkId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
