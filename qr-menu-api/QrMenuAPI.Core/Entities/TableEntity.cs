namespace QrMenuAPI.Core.Entities;

public class TableEntity
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;

    public int EstablishmentId { get; set; }
    public EstablishmentEntity Establishment { get; set; } = null!;
}
