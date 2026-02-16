namespace QrMenuAPI.Client.Models;

public class CreateOrderRequest
{
    public int TableId { get; set; }
    public string? CustomerFullName { get; set; }
    public string? Comment { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];

    public bool IsValid()
    {
        if (TableId <= 0)
            return false;

        if (Items == null || Items.Count == 0)
            return false;

        return Items.All(i => i != null && i.IsValid());
    }
}
