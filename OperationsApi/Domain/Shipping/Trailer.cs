namespace OperationsApi.Domain.Shipping;

internal sealed class Trailer
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public List<Shipment> Shipments { get; set; } = [];
}