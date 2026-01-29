namespace QrMenuAPI.Admin.Models.Network;

public class UpdateNetworkRequest
{
    public string Name { get; set; } = null!;

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Name);
}

