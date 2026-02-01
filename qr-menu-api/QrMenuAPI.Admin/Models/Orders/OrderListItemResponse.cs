using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Orders;

public class OrderListItemResponse
{
    public int Id { get; set; }
    public int OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public int EstablishmentId { get; set; }
    public string EstablishmentName { get; set; } = null!;

    public int? TableId { get; set; }
    public string? TableNumber { get; set; }

    public int? WaiterId { get; set; }
    public string? WaiterFullName { get; set; }

    public List<OrderItemShortResponse> Items { get; set; } = [];
}

