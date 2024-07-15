using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingTask : WarehouseTask
{
    public Dock StagingDock { get; private set; } = default!;
    public Area StagingArea { get; private set; } = default!;
    public Pallet Pallet { get; private set; } = default!;
    public PickingLine? CurrentPickLine { get; private set; }
    public bool IsStaging { get; private set; }
    public List<PickingLine> PickLines { get; private set; } = [];

    internal override bool StartWith( Employee employee )
    {
        if (!base.StartWith( employee ))
            return false;
        Pallet = Pallet.NewEmpty( employee );
        CurrentPickLine = PickLines.FirstOrDefault();
        return true;
    }
    internal bool StartPickingLocation( Guid rackingId, Guid palletId )
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
    internal bool PickItem( Guid itemId )
    {
        bool picked = CurrentPickLine is not null
            && CurrentPickLine.PickItem( Employee, itemId );
        return picked;
    }
    internal bool FinishPickingLocation( Guid rackingId, Guid palletId )
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
    internal bool StagePick( Guid areaId )
    {
        bool staged = IsStaging
            && StagingArea.Id == areaId
            && CurrentPickLine is null
            && Employee.StagePallet( areaId, Guid.Empty );

        if (staged)
            IsFinished = true;
        
        return staged;
    }
}