using OperationsDomain.Warehouse.Infrastructure.Units;
using OperationsDomain.Warehouse.Operations;
using OperationsDomain.Warehouse.Operations.Picking.Models;

namespace OperationsDomain.Warehouse.Employees.Models;

public sealed class PickingEmployee : Employee
{
    public PickingEmployee( Guid id, string name, Pallet? palletEquipped, PickingTask? task )
        : base( id, name, palletEquipped, task ) { }
    public PickingTask? PickingTask => 
        TaskAs<PickingTask>();
    
    public bool StartPicking( PickingOperations picking, Guid taskId )
    {
        if (Task is not null)
            return false;

        var task = picking.GetPendingTask( taskId );
        
        return task is not null
            && StartTask( task )
            && picking.ActivateTask( task );
    }
    public bool StartPickingPallet( Guid rackingId, Guid palletId )
    {
        var pallet = PickingTask?.StartPickingPallet( rackingId, palletId );

        return pallet is not null
            && pallet.Racking is not null
            && UnRackPallet( pallet.Racking, pallet );
    }
    public bool FinishPickingPallet( Guid areaId )
    {
        var pallet = PickingTask?.FinishPickingPallet( areaId );

        return pallet is not null
            && PickingTask is not null
            && StagePallet( PickingTask.StagingArea, pallet );
    }
    public bool FinishPicking( PickingOperations picking )
    {
        return PickingTask is not null
            && picking.HandleCompletedTask( PickingTask )
            && EndTask();
    }
}