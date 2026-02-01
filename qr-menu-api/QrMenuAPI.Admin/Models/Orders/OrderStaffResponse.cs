using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Orders;

public class OrderStaffResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public OrderStaffRole Role { get; set; }
    public DateTime AssignedAt { get; set; }
}
