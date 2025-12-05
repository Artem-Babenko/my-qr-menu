using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class UserSessionConfig : IEntityTypeConfiguration<UserSessionEntity>
{
    public void Configure(EntityTypeBuilder<UserSessionEntity> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(s => s.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.LastActivityAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.ExpiresAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.IpAddress)
            .HasMaxLength(50);

        builder.Property(s => s.UserAgent)
            .HasMaxLength(500);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
