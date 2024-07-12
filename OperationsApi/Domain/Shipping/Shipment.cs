using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Domain.Shipping;

internal sealed class Shipment
{
    public Guid Id { get; set; }
    public Guid? TrailerId { get; set; }
    public Trailer? Trailer { get; set; }
    public List<Pallet> Pallets { get; set; } = [];
}