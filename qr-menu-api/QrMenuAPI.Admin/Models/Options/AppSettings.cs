namespace QrMenuAPI.Admin.Models.Options;

public class AppSettings
{
    public string JwtSecret { get; set; } = null!;
    public TimeSpan JwtLifeTime { get; set; }
}
