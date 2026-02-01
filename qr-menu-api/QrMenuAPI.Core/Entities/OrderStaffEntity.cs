using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Core.Entities;

public class OrderStaffEntity
{
    public int Id { get; set; }
    public OrderStaffRole Role { get; set; }
    public DateTime AssignedAt { get; set; }

    public int OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;

    public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}
