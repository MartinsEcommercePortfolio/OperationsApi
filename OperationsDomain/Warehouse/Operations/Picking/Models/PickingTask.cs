using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingTask : WarehouseTask
{
    internal PickingTask( Guid warehouseOrderId, Dock dock, Area area, List<Pallet> pallets )
    {
        Id = Guid.NewGuid();
        WarehouseOrderId = warehouseOrderId;
        StagingDock = dock;
        StagingArea = area;
        Pallets = pallets;
        StagedPallets = [];
    }
    
    public Guid WarehouseOrderId { get; private set; }
    public Dock StagingDock { get; private set; }
    public Area StagingArea { get; private set; }
    public Pallet? CurrentPallet { get; private set; }
    public List<Pallet> Pallets { get; private set; }
    public List<Pallet> StagedPallets { get; private set; }

    internal Pallet? StartPickingPallet( Guid rackingId, Guid palletId )
    {
        if (CurrentPallet is not null)
            return null;

        CurrentPallet = Pallets.FirstOrDefault( p => 
            p.Id == palletId && p.Racking!.Id == rackingId );

        if (CurrentPallet is null || StagedPallets.Contains( CurrentPallet ))
            return null;
        
        return CurrentPallet;
    }
    internal Pallet? FinishPickingPallet( Guid areaId )
    {
        if (CurrentPallet is null || areaId != StagingArea.Id)
            return null;
        
        StagedPallets.Add( CurrentPallet );
        var temp = CurrentPallet;
        CurrentPallet = null;
        return temp;
    }
}