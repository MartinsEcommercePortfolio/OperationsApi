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
    public bool StartPickingLine( Guid lineId, Guid rackingId, Guid palletId )
    {
        bool canPick = !IsStaging && CurrentPickLine is null;
        if (!canPick)
            return false;

        CurrentPickLine = PickLines.FirstOrDefault( p =>
            p.Id == lineId &&
            p.Racking.Id == rackingId &&
            p.Racking.Pallet is not null &&
            p.Racking.Pallet.Id == palletId );

        return CurrentPickLine is not null
            ? CurrentPickLine.StartPicking( palletId, rackingId, Employee )
            : false;
    }
    public bool PickItem( Guid itemId )
    {
        bool picked = CurrentPickLine is not null
            && CurrentPickLine.PickItem( Employee, itemId );
        
        return picked;
    }
    public bool FinishPickingLocation( Guid lineId )
    {
        bool finished = CurrentPickLine is not null
            && CurrentPickLine.Id == lineId
            && CurrentPickLine.IsComplete();

        if (finished)
            CurrentPickLine = null;

        return finished;
    }
    public bool StagePick( Guid areaId )
    {
        bool staged = IsStaging
            && StagingArea.Id == areaId
            && CurrentPickLine is null
            && Employee.StagePallet( StagingArea, Pallet );

        if (staged)
            IsFinished = true;
        
        return staged;
    }
}