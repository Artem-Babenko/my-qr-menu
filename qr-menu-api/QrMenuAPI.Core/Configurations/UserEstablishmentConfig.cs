using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class UserEstablishmentConfig : IEntityTypeConfiguration<UserEstablishmentEntity>
{
    public void Configure(EntityTypeBuilder<UserEstablishmentEntity> builder)
    {
        builder.ToTable("user_establishments");

        builder.HasKey(ue => new { ue.UserId, ue.EstablishmentId });

        builder.HasOne(ue => ue.User)
            .WithMany(u => u.UserEstablishment)
            .HasForeignKey(ue => ue.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Establishment)
            .WithMany(e => e.UserEstablishment)
            .HasForeignKey(ue => ue.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Role)
            .WithMany(r => r.UserEstablishment)
            .HasForeignKey(ue => ue.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
