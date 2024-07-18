using OperationsDomain.Warehouse.Operations.Picking.Models;
using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Employees.Models.Variants;

public sealed class PickingEmployee : Employee
{
    public PickingTask? PickingTask => TaskAs<PickingTask>();

    // PICKING
    public bool StartPicking( PickingOperations picking, Guid taskId )
    {
        if (PickingTask is not null)
            return false;

        var task = picking.GetPendingTask( taskId );
        
        return task is not null
            && StartTask( task )
            && picking.ActivateTask( task );
    }
    public bool StartPickingLine( Guid lineId, Guid rackingId )
    {
        var pickLine = PickingTask?.SetPickingLine( lineId );
        return pickLine is not null
            && pickLine.StartPicking( this, rackingId );
    }
    public bool PickItem( ReplenishingOperations replenishing, Guid itemId )
    {
        return PickingTask?.CurrentPickLine is not null
            && PickingTask.CurrentPickLine.PickItem( this, replenishing, itemId );
    }
    public bool FinishPickingLine( Guid lineId )
    {
        return PickingTask is not null
            && PickingTask.FinishPickingLocation( lineId );
    }
    public bool FinishPicking( PickingOperations picking, Guid areaId )
    {
        return PickingTask is not null
            && PickingTask.InitializeStaging( areaId )
            && StagePallet( PickingTask.StagingArea, PickingTask.Pallet )
            && picking.HandleCompletedTask( PickingTask )
            && EndTask();
    }
}