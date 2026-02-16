namespace QrMenuAPI.Client.Models;

public class OrderItemResponse
{
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal LineTotal { get; set; }
}
