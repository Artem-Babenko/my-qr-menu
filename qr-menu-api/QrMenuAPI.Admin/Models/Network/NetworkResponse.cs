namespace QrMenuAPI.Admin.Models.Network;

public class NetworkResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public IEnumerable<EstablishmentResponse> Establishments { get; set; } = [];
}
