namespace QrMenuAPI.Admin.Models.Tables;

public class CreateTableRequest
{
    public int EstablishmentId { get; set; }
    public string Number { get; set; } = null!;

    public bool IsValid()
    {
        if (EstablishmentId <= 0) return false;
        if (string.IsNullOrWhiteSpace(Number)) return false;
        return Number.Trim().Length <= 50;
    }
}

