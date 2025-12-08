using QrMenuAPI.Core;
using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Services.Invitations;

public class InvitationService(AppDbContext db) : IInvitationService
{
    public async Task AcceptInvitation(InvitationEntity invitation, UserEntity user)
    {
        if (user.NetworkId.HasValue)
            throw new InvalidOperationException("User already belongs to a network.");

        user.NetworkId = invitation.Establishment.NetworkId;

        var userEstablishment = new UserEstablishmentEntity
        {
            UserId = user.Id,
            EstablishmentId = invitation.EstablishmentId,
            RoleId = invitation.RoleId
        };

        invitation.Status = InvitationStatus.Accepted;

        db.UserEstablishments.Add(userEstablishment);
        db.Users.Update(user);
        db.Invitations.Update(invitation);

        await db.SaveChangesAsync();
    }
}
