namespace QrMenuAPI.Admin.Models.Invitation;

public class InvitationForExisting : InvitationRequest
{
    public int TargetUserId { get; set; }

    public override bool IsValid() =>
        base.IsValid() &&
        TargetUserId > 0;
}
