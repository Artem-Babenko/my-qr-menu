namespace QrMenuAPI.Admin.Models.Categories;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int NetworkId { get; set; }
}
