using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Orders;

public class OrderStatusHistoryResponse
{
    public int Id { get; set; }
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public int ChangedByUserId { get; set; }
    public string ChangedByUserFullName { get; set; } = null!;
}

