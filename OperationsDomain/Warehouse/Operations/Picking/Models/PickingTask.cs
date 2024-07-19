using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingTask : WarehouseTask
{
    PickingTask(
        Guid id, Employee? employee, bool isStarted, bool isFinished, Guid warehouseOrderId, Dock dock, Area area, List<Pallet> pallets )
        : base( id, employee, isStarted, isFinished )
    {
        WarehouseOrderId = warehouseOrderId;
        StagingDock = dock;
        StagingArea = area;
        Pallets = pallets;
        StagedPallets = [];
    }

    public static PickingTask New( Guid warehouseOrderId, Dock dock, Area area, List<Pallet> pallets ) =>
        new( Guid.NewGuid(), null, false, false, warehouseOrderId, dock, area, pallets );
    
    public Guid WarehouseOrderId { get; private set; }
    public Dock StagingDock { get; private set; }
    public Area StagingArea { get; private set; }
    public Pallet? CurrentPallet { get; private set; }
    public List<Pallet> Pallets { get; private set; }
    public List<Pallet> StagedPallets { get; private set; }

    internal override bool StartWith( Employee employee )
    {
        return base.StartWith( employee )
            && StagingDock.AssignTo( employee )
            && Pallets.All( p =>
                p.AssignTo( employee ) &&
                p.Racking is not null &&
                p.Racking.AssignTo( employee ) );
    }
    internal override bool CleanUp( Employee employee )
    {
        return base.CleanUp( employee )
            && StagingDock.UnAssignFrom( employee )
            && Pallets.All( p =>
                p.UnAssignFrom( employee ) );
    }
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