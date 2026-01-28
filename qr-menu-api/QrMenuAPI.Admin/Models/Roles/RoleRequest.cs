using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Roles;

public class RoleRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int NetworkId { get; set; }
    public IEnumerable<PermissionType> Permissions { get; set; } = [];

    public bool IsValid() =>
        NetworkId > 0 &&
        !string.IsNullOrWhiteSpace(Name);
}
