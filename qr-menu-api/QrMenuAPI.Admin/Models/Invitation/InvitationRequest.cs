namespace QrMenuAPI.Admin.Models.Invitation;

public class InvitationRequest
{
    public int EstablishmentId { get; set; }
    public int RoleId { get; set; }

    public virtual bool IsValid() =>
        EstablishmentId > 0 &&
        RoleId > 0;
}
