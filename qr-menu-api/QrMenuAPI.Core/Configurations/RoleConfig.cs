using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class RoleConfig : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.HasMany(r => r.UserEstablishment)
            .WithOne(ue => ue.Role)
            .HasForeignKey(ue => ue.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
