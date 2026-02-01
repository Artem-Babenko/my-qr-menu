namespace QrMenuAPI.Admin.Models.Orders;

public class OrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public bool IsValid() =>
        ProductId > 0 &&
        Quantity is >= 1 and <= 99;
}
