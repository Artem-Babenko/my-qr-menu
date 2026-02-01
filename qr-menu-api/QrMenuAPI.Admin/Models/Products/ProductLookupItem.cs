namespace QrMenuAPI.Admin.Models.Products;

public class ProductLookupItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public decimal Price { get; set; }
}
