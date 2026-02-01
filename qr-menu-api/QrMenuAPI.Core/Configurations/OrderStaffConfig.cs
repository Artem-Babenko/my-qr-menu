using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class OrderStaffConfig : IEntityTypeConfiguration<OrderStaffEntity>
{
    public void Configure(EntityTypeBuilder<OrderStaffEntity> builder)
    {
        builder.ToTable("order_staff");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Role)
            .IsRequired();

        builder.Property(s => s.AssignedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.HasOne(s => s.Order)
            .WithMany(o => o.Staff)
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.User)
            .WithMany(u => u.AssignedOrders)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.OrderId, s.UserId, s.Role })
            .IsUnique();
    }
}
