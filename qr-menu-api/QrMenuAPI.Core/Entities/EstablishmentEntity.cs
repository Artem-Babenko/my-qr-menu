namespace QrMenuAPI.Core.Entities;

public class EstablishmentEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public int NetworkId { get; set; }
    public NetworkEntity Network { get; set; } = null!;

    public ICollection<UserEstablishmentEntity> UserEstablishment { get; set; } = [];
    public ICollection<InvitationEntity> Invitations { get; set; } = [];
    public ICollection<TableEntity> Tables { get; set; } = [];
    public ICollection<OrderEntity> Orders { get; set; } = [];
}
