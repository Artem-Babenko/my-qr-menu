namespace QrMenuAPI.Core.Entities;

public class RoleEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public int NetworkId { get; set; }
    public NetworkEntity Network { get; set; } = null!;

    public ICollection<UserEstablishmentEntity> UserEstablishment { get; set; } = [];
}
