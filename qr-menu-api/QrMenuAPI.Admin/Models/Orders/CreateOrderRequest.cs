namespace QrMenuAPI.Admin.Models.Orders;

public class CreateOrderRequest
{
    public int EstablishmentId { get; set; }
    public int? TableId { get; set; }
    public int? WaiterId { get; set; }
    public string? CustomerFullName { get; set; }
    public string? Comment { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];

    public bool IsValid()
    {
        if (EstablishmentId <= 0)
            return false;

        if (TableId.HasValue && TableId.Value <= 0)
            return false;

        if (WaiterId.HasValue && WaiterId.Value <= 0)
            return false;

        if (Items == null || Items.Count == 0)
            return false;

        return Items.All(i => i != null && i.IsValid());
    }
}

