namespace QrMenuAPI.Admin.Models.Categories;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Name)) return false;

        var name = Name.Trim();
        if (name.Length > 200) return false;

        if (Description != null && Description.Length > 1000) return false;

        return true;
    }
}

