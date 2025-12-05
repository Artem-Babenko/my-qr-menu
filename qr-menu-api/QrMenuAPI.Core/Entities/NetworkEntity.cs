namespace QrMenuAPI.Core.Entities;

public class NetworkEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<EstablishmentEntity> Establishments { get; set; } = [];
    public ICollection<RoleEntity> Roles { get; set; } = [];
    public ICollection<UserEntity> Users { get; set; } = [];
}
