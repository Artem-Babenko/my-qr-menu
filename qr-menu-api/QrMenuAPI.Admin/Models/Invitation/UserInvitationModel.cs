using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Models.Invitation;

public class UserInvitationModel
{
    public Guid Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public int NetworkId { get; set; }
    public int EstablishmentId { get; set; }
    public string EstablishmentName { get; set; } = null!;
    public string EstablishmentAddress { get; set; } = null!;
    public InvitationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
