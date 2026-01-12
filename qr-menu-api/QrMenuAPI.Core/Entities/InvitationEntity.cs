using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Core.Entities;

public class InvitationEntity
{
    public Guid Id { get; set; }

    public string? Phone { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }

    public int? TargetUserId { get; set; }
    public UserEntity? TargetUser { get; set; }

    public int EstablishmentId { get; set; }
    public EstablishmentEntity Establishment { get; set; } = null!;

    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;

    public InvitationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
}
