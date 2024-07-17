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
    internal PickingLine? SetPickingLine( Guid lineId )
    {
        if (IsStaging || CurrentPickLine is not null)
            return null;

        CurrentPickLine = PickLines.FirstOrDefault( p =>
            p.Id == lineId );

        return CurrentPickLine;
    }
    internal bool FinishPickingLocation( Guid lineId )
    {
        bool finished = CurrentPickLine is not null
            && CurrentPickLine.Id == lineId
            && CurrentPickLine.IsComplete();

        if (finished)
            CurrentPickLine = null;

        return finished;
    }
    internal bool InitializeStaging( Guid areaId )
    {
        bool staged = IsStaging
            && StagingArea.Id == areaId
            && CurrentPickLine is null;

        if (staged)
            IsFinished = true;
        
        return staged;
    }
}