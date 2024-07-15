using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Models;

public sealed class PickingTask : WarehouseTask
{
    public Dock StagingDock { get; set; } = default!;
    public Area StagingArea { get; set; } = default!;
    public Pallet Pallet { get; set; } = default!;
    public PickingLine? CurrentPickLine { get; set; }
    public bool IsStaging { get; set; }
    public List<PickingLine> PickLines { get; set; } = [];
    
    public override bool Start( Employee employee )
    {
        if (!base.Start( employee ))
            return false;
        Pallet = Pallet.NewEmpty( employee );
        CurrentPickLine = PickLines.FirstOrDefault();
        return true;
    }
    public bool StartPickingLocation( Guid palletId, Guid rackingId )
    {
        if (CurrentPickLine is null || CurrentPickLine.IsComplete())
            return false;

        CurrentPickLine = PickLines.FirstOrDefault( p =>
            p.Racking.Id == rackingId &&
            p.Racking.Pallet is not null &&
            p.Racking.Pallet.Id == palletId );

        return CurrentPickLine?
            .ConfirmPickLocation( palletId, rackingId ) ?? false;
    }
    public bool PickItem( Guid itemId )
    {
        bool picked = CurrentPickLine is not null
            && CurrentPickLine.PickItem( Employee, itemId );
        return picked;
    }
    public bool FinishPickingLocation( Guid palletId, Guid rackingId )
    {
        if (CurrentPickLine is null || CurrentPickLine.IsComplete())
            return false;

        CurrentPickLine = PickLines.FirstOrDefault( p =>
            p.Racking.Id == rackingId &&
            p.Racking.Pallet is not null &&
            p.Racking.Pallet.Id == palletId );

        return CurrentPickLine?
            .ConfirmPickLocation( palletId, rackingId ) ?? false;
    }
    public bool StagePick( Guid areaId )
    {
        bool staged = IsStaging
            && StagingArea.Id == areaId
            && CurrentPickLine is null
            && StagingArea.TakePallet( Pallet )
            && Pallet.PlaceInArea( StagingArea );
        if (staged)
            Employee.FinishTask();
        return staged;
    }
}