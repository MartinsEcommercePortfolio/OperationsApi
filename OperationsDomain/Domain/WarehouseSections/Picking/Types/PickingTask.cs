using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public sealed class PickingTask : WarehouseTask
{
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
    public PickingLine? GetNextPick()
    {
        if (CurrentPickLine is null || !CurrentPickLine.IsComplete())
            return null;
        
        int nextIndex = PickLines.IndexOf( CurrentPickLine );
        if (nextIndex >= PickLines.Count)
        {
            CurrentPickLine = null;
            IsStaging = true;
            return null;
        }
        CurrentPickLine = PickLines[nextIndex];
        return CurrentPickLine;
    }
    public int? ConfirmPickLocation( Guid rackingId )
    {
        if (CurrentPickLine is null || CurrentPickLine.IsComplete())
            return null;
        return CurrentPickLine
            .ConfirmPickLocation( rackingId );
    }
    public int? PickItem( Guid itemId )
    {
        bool picked = CurrentPickLine is not null
            && CurrentPickLine.PickItem( Employee, itemId );
        return picked
            ? CurrentPickLine!.ItemsRemainingInPick()
            : null;
    }
    public bool StagePick( Guid areaId )
    {
        bool staged = IsStaging
            && CurrentPickLine is null
            && StagingArea.StagePallet( Pallet )
            && Pallet.PutIn( StagingArea );
        if (staged)
            Employee.FinishTask();
        return staged;
    }
}