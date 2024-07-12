using OperationsApi.Domain.Shipping;

namespace OperationsApi.Domain.Warehouses.Receiving;

internal sealed class ReceivingTask : WarehouseTask
{
    public Trailer Trailer { get; set; } = null!;
    public Dock Dock { get; set; } = null!;
    public Area Area { get; set; } = null!;
    public Shipment Shipment { get; set; } = null!;
    public List<Pallet> Pallets { get; set; } = null!;
    public Guid TrailerId { get; set; }
    public Guid DockId { get; set; }
    public Guid AreaId { get; set; }
    public Guid ShipmentId { get; set; }
}