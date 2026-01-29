namespace QrMenuAPI.Admin.Models.Network;

public class UpdateEstablishmentRequest
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(Address);
}

