namespace QrMenuAPI.Core.Entities;

public class ProductPriceEntity
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;

    public int EstablishmentId { get; set; }
    public EstablishmentEntity Establishment { get; set; } = null!;
}
