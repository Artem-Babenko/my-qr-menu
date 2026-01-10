namespace QrMenuAPI.Admin.Models.Invitation;

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
