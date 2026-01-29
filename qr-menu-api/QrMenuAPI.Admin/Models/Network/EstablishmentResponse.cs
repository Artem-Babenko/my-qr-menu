namespace QrMenuAPI.Admin.Models.Network;

public class EstablishmentResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int UsersCount { get; set; }
}
