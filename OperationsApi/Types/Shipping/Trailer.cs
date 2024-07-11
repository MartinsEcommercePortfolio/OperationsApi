namespace OperationsApi.Types.Shipping;

internal sealed class Trailer
{
    public Guid Id { get; set; }
    public List<Shipment> Shipments { get; set; } = [];
}