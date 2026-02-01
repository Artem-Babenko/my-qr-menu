namespace QrMenuAPI.Admin.Models.Products;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int NetworkId { get; set; }
    public int CategoryId { get; set; }

    public List<ProductPriceItemModel> Prices { get; set; } = [];
}

