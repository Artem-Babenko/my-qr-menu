using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Core.Entities;

public class OrderStatusHistoryEntity
{
    public int Id { get; set; }
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public DateTime ChangedAt { get; set; }

    public int OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;

    public int ChangedByUserId { get; set; }
    public UserEntity ChangedByUser { get; set; } = null!;
}
