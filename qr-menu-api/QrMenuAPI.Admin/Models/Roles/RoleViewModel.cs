using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Roles;

public class RoleViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public IEnumerable<PermissionType> Permissions { get; set; } = [];
    public int NumberOfUsers { get; set; }
}
