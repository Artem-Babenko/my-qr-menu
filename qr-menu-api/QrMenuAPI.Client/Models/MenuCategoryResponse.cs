namespace QrMenuAPI.Client.Models;

public class MenuCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public List<MenuProductResponse> Products { get; set; } = [];
}
