using OperationsDomain.Operations.Picking.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Employees.Models;

public sealed class PickingEmployee : Employee
{
    PickingEmployee( Guid id, string name, Pallet? palletEquipped, PickingTask? task )
        : base( id, name, palletEquipped, task ) { }
    
    public PickingTask? PickingTask => 
        TaskAs<PickingTask>();
    
    public bool StartPicking( PickingOperations picking, Guid taskId )
    {
        if (Task is not null)
            return false;

        var task = picking.GetTask( taskId );
        
        return task is not null
            && StartTask( task )
            && picking.ActivateTask( task );
    }
    public bool FinishPicking( PickingOperations picking )
    {
        return PickingTask is not null
            && picking.HandleCompletedTask( PickingTask )
            && EndTask();
    }
    public bool PickPallet( Guid rackingId, Guid palletId )
    {
        var pallet = PickingTask?.Pick( rackingId, palletId );

        return pallet is not null
            && pallet.Racking is not null
            && pallet.Product.Pick()
            && UnRackPallet( pallet.Racking, pallet );
    }
    public bool StagePallet( Guid areaId )
    {
        var pallet = PickingTask?.Stage( areaId );

        return pallet is not null
            && PickingTask is not null
            && StagePallet( PickingTask.StagingArea, pallet );
    }
}