using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class InvitationConfig : IEntityTypeConfiguration<InvitationEntity>
{
    public void Configure(EntityTypeBuilder<InvitationEntity> builder)
    {
        builder.ToTable("invitations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Phone)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(128);

        builder.Property(x => x.Surname)
            .HasMaxLength(128);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.ExpiredAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.HasOne(x => x.TargetUser)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Establishment)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.Phone, x.EstablishmentId })
            .IsUnique();
    }
}
