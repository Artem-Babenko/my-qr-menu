namespace QrMenuAPI.Admin.Models.Invitation;

public class InvitationRequest
{
    public int EstablishmentId { get; set; }
    public int RoleId { get; set; }

    public virtual bool IsValid() =>
        EstablishmentId > 0 &&
        RoleId > 0;
}

public class InvitationForExisting : InvitationRequest
{
    public int TargetUserId { get; set; }

    public override bool IsValid() =>
        base.IsValid() &&
        TargetUserId > 0;
}

public class InvitationForNew : InvitationRequest
{
    public string Phone { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;

    public override bool IsValid() =>
        base.IsValid() &&
        !string.IsNullOrWhiteSpace(Phone) &&
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(Surname);
}
