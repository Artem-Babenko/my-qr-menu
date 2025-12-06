using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Core.Entities;

public class RolePermissionEntity
{
    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
    public PermissionType PermissionType { get; set; }
}
