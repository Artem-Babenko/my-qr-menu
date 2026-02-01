namespace QrMenuAPI.Admin.Models.Products;

public class ProductPriceItemModel
{
    public int EstablishmentId { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }

    public bool IsValid()
    {
        if (EstablishmentId <= 0) return false;
        if (Price < 0) return false;
        return true;
    }
}

