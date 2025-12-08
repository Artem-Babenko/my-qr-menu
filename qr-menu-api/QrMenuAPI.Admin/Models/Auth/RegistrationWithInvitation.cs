namespace QrMenuAPI.Admin.Models.Auth;

public class RegistrationWithInvitation : RegistrationRequest
{
    public Guid InvitationId { get; set; }
}
