namespace QrMenuAPI.Admin.Models.Tables;

public class UpdateTableRequest
{
    public string Number { get; set; } = null!;

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Number)) return false;
        return Number.Trim().Length <= 50;
    }
}

