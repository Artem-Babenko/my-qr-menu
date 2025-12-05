namespace QrMenuAPI.APP.Models.Auth;

public class LoginRequest
{
    public string Phone { get; set; } = null!;
    public string Password { get; set; } = null!;

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Phone) &&
        !string.IsNullOrWhiteSpace(Password);
}
