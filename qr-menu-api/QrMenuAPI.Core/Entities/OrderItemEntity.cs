namespace QrMenuAPI.Core.Entities;

public class OrderItemEntity
{
    public int Id { get; set; }
    public string ProductNameSnapshot { get; set; } = null!;
    public string CategoryNameSnapshot { get; set; } = null!;
    public decimal PriceSnapshot { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public int OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;

    public int? ProductId { get; set; }
    public ProductEntity? Product { get; set; }
}
