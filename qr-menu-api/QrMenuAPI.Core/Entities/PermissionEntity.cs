using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Core.Entities;

public class PermissionEntity
{
    public PermissionType Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
