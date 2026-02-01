using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core.Configurations;

internal class OrderConfig : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.OrderNumber)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.Source)
            .IsRequired();

        builder.Property(o => o.TotalSum)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.IsPaid)
            .IsRequired();

        builder.Property(o => o.CustomerFullName)
            .HasMaxLength(200);

        builder.Property(o => o.Comment)
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(o => o.ClosedAt)
            .HasColumnName("timestamp with time zone");

        builder.HasOne(o => o.Establishment)
            .WithMany(es => es.Orders)
            .HasForeignKey(o => o.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Table)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.CreatedByUser)
            .WithMany(u => u.OrdersCreated)
            .HasForeignKey(o => o.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Staff)
            .WithOne(s => s.Order)
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.StatusHistory)
            .WithOne(h => h.Order)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
