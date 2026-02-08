using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Client.Models;

public class OrderResponse
{
    public int Id { get; set; }
    public int OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalSum { get; set; }
    public string? CustomerFullName { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = [];
}
