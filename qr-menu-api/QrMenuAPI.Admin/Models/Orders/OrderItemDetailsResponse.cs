namespace QrMenuAPI.Admin.Models.Orders;

public class OrderItemDetailsResponse
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
