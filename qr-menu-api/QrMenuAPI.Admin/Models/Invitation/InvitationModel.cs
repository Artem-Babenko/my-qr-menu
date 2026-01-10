using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Invitation;

public class InvitationModel
{
    public Guid Id { get; set; }
    public string? Phone { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Surname { get; set; } = null!;
    public int RoleId { get; set; }
    public InvitationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
}
