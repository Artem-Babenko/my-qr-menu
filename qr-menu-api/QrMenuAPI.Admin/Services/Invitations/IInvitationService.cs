using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Services.Invitations;

public interface IInvitationService
{
    Task AcceptInvitation(InvitationEntity invitation, UserEntity user);
}
