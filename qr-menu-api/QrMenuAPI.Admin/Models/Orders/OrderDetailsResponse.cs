using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Orders;

public class OrderDetailsResponse
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
    public string EstablishmentName { get; set; } = null!;

    public int? TableId { get; set; }
    public string? TableNumber { get; set; }

    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserFullName { get; set; }

    public List<OrderItemDetailsResponse> Items { get; set; } = [];
    public List<OrderStaffResponse> Staff { get; set; } = [];
    public List<OrderStatusHistoryResponse> StatusHistory { get; set; } = [];
}
