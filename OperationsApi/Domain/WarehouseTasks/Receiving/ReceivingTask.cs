using OperationsApi.Domain.Shipping;
using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Domain.WarehouseTasks.Receiving;

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

    public bool ReceiveTrailerPallet( Employee employee, Guid palletId )
    {
        Pallet? pallet = Pallets.FirstOrDefault( p => p.Id == palletId );
        return pallet is not null && pallet.Receive( employee );
    }
    public bool StageReceivedPallet( Employee employee, Guid palletId, Guid areaId )
    {
        if (areaId != Area.Id)
            return false;
        Pallet? pallet = Pallets.FirstOrDefault( p => p.Id == palletId );
        return pallet is not null && pallet.Stage( employee, Area );
    }
}