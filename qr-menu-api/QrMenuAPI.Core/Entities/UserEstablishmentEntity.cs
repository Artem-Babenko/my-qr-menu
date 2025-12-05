namespace QrMenuAPI.Core.Entities;

public class UserEstablishmentEntity
{
    public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public int EstablishmentId { get; set; }
    public EstablishmentEntity Establishment { get; set; } = null!;

    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
}
