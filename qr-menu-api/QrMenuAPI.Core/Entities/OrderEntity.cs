using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Core.Entities;

public class OrderEntity
{
    public int Id { get; set; }
    public int OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public OrderSource Source { get; set; }
    public decimal TotalSum { get; set; }
    public bool IsPaid { get; set; }
    public string? CustomerFullName { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public int EstablishmentId { get; set; }
    public EstablishmentEntity Establishment { get; set; } = null!;

    public int? TableId { get; set; }
    public TableEntity? Table { get; set; }

    public int? CreatedByUserId { get; set; }
    public UserEntity? CreatedByUser { get; set; }

    public ICollection<OrderItemEntity> Items { get; set; } = [];
    public ICollection<OrderStaffEntity> Staff { get; set; } = [];
    public ICollection<OrderStatusHistoryEntity> StatusHistory { get; set; } = [];
}
