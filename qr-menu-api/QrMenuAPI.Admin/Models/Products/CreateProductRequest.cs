namespace QrMenuAPI.Admin.Models.Products;

public class CreateProductRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int NetworkId { get; set; }
    public int CategoryId { get; set; }

    public List<ProductPriceItemModel> Prices { get; set; } = [];

    public bool IsValid()
    {
        if (NetworkId <= 0) return false;
        if (CategoryId <= 0) return false;

        if (string.IsNullOrWhiteSpace(Name)) return false;
        var name = Name.Trim();
        if (name.Length > 200) return false;

        if (string.IsNullOrWhiteSpace(Description)) return false;
        if (Description.Length > 1000) return false;

        if (Prices.Any(p => !p.IsValid())) return false;

        return true;
    }
}

