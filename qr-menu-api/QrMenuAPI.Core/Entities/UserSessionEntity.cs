namespace QrMenuAPI.Core.Entities;

public class UserSessionEntity
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
