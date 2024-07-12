namespace OperationsApi.Domain.Shipping;

internal sealed class Delivery
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }
    public List<Load> Loads { get; set; } = [];
}