namespace QrMenuAPI.Admin.Models.Orders;

public class UpdateOrderRequest
{
    public int? TableId { get; set; }
    public string? CustomerFullName { get; set; }
    public string? Comment { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];

    public bool IsValid()
    {
        if (TableId.HasValue && TableId.Value <= 0)
            return false;

        if (Items == null || Items.Count == 0)
            return false;

        return Items.All(i => i != null && i.IsValid());
    }
}

