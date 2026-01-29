namespace QrMenuAPI.Admin.Models.User;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int? NetworkId { get; set; }

    public List<UserEstablishmentAccessModel> Accesses { get; set; } = [];
}
