namespace QrMenuAPI.Client.Models;

public class TableResponse
{
    public int TableId { get; set; }
    public string TableNumber { get; set; } = null!;
    public int EstablishmentId { get; set; }
    public string EstablishmentName { get; set; } = null!;
    public string EstablishmentAddress { get; set; } = null!;
}
