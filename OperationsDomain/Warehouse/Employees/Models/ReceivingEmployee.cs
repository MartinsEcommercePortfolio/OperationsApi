using OperationsDomain.Warehouse.Infrastructure.Units;
using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsDomain.Warehouse.Employees.Models;

public sealed class ReceivingEmployee : Employee
{
    public ReceivingEmployee( Guid id, string name, Pallet? palletEquipped, ReceivingTask? task )
        : base( id, name, palletEquipped, task ) { }
    public ReceivingTask? ReceivingTask => TaskAs<ReceivingTask>();

    public bool StartReceiving( ReceivingOperations receiving, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        if (Task is not null)
            return false;

        var task = receiving.GetReceivingTask( taskId );
        
        return task is not null
            && StartTask( task )
            && task.VerifyStart( trailerId, dockId, areaId )
            && receiving.ActivateReceivingTask( task );
    }
    public bool FinishReceiving( ReceivingOperations receiving )
    {
        return ReceivingTask is not null
            && ReceivingTask.CleanUp( this )
            && EndTask()
            && receiving.CompleteTask( ReceivingTask );
    }
    public bool StartReceivingPallet( Guid trailerId, Guid palletId )
    {
        var pallet = ReceivingTask?.StartReceivingPallet( trailerId, palletId );

        return pallet is not null
            && ReceivingTask is not null
            && UnloadPallet( ReceivingTask.Trailer, pallet );
    }
    public bool FinishReceivingPallet( Guid areaId, Guid palletId )
    {
        if (ReceivingTask is null)
            return false;
        
        return ReceivingTask?.CurrentPallet is not null
            && StagePallet( ReceivingTask.Area, ReceivingTask.CurrentPallet )
            && ReceivingTask.FinishReceivingPallet( areaId, palletId );
    }
}