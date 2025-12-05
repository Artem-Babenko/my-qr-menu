namespace QrMenuAPI.APP.Models.Auth;

public class RegistrationRequest
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(Surname) &&
        !string.IsNullOrWhiteSpace(Phone) && // TODO: додати регекс
        !string.IsNullOrWhiteSpace(Email) && // TODO: додати регекс
        !string.IsNullOrWhiteSpace(Password);
}
