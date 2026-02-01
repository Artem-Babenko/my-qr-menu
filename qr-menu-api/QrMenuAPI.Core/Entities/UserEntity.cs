namespace QrMenuAPI.Core.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public int? NetworkId { get; set; }
    public NetworkEntity? Network { get; set; }

    public ICollection<UserEstablishmentEntity> UserEstablishment { get; set; } = [];
    public ICollection<UserSessionEntity> Sessions { get; set; } = [];
    public ICollection<InvitationEntity> Invitations { get; set; } = [];
    public ICollection<OrderEntity> OrdersCreated { get; set; } = [];
    public ICollection<OrderStaffEntity> AssignedOrders { get; set; } = [];
    public ICollection<OrderStatusHistoryEntity> StatusChanges { get; set; } = [];
}
