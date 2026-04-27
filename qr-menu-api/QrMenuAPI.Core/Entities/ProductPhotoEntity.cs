namespace QrMenuAPI.Core.Entities;

public class ProductPhotoEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int SortOrder { get; set; }
}
