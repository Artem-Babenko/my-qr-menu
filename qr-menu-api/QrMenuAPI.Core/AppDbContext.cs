using Microsoft.EntityFrameworkCore;
using QrMenuAPI.Core.Configurations;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Core;

public class AppDbContext : DbContext
{
    public DbSet<NetworkEntity> Networks { get; set; }
    public DbSet<EstablishmentEntity> Establishments { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<UserEstablishmentEntity> UserEstablishments { get; set; }
    public DbSet<UserSessionEntity> UserSessions { get; set; }
    public DbSet<InvitationEntity> Invitations { get; set; }
    public DbSet<TableEntity> Tables { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductPriceEntity> Prices { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }
    public DbSet<OrderStaffEntity> OrderStaffs { get; set; }
    public DbSet<OrderStatusHistoryEntity> OrderStatusHistories { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NetworkConfig());
        modelBuilder.ApplyConfiguration(new EstablishmentConfig());
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new RoleConfig());
        modelBuilder.ApplyConfiguration(new RolePermissionConfig());
        modelBuilder.ApplyConfiguration(new UserEstablishmentConfig());
        modelBuilder.ApplyConfiguration(new UserSessionConfig());
        modelBuilder.ApplyConfiguration(new InvitationConfig());
        modelBuilder.ApplyConfiguration(new TableConfig());
        modelBuilder.ApplyConfiguration(new ProductConfig());
        modelBuilder.ApplyConfiguration(new ProductPriceConfig());
        modelBuilder.ApplyConfiguration(new CategoryConfig());
        modelBuilder.ApplyConfiguration(new OrderConfig());
        modelBuilder.ApplyConfiguration(new OrderItemConfig());
        modelBuilder.ApplyConfiguration(new OrderStaffConfig());
        modelBuilder.ApplyConfiguration(new OrderStatusHistoryConfig());
        base.OnModelCreating(modelBuilder);
    }
}
