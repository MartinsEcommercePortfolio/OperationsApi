using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsDomain.Warehouse.Employees.Models.Variants;

public sealed class ReceivingEmployee : Employee
{
    public ReceivingTask? ReceivingTask => TaskAs<ReceivingTask>();

    public bool StartReceiving( ReceivingOperations receiving, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        if (ReceivingTask is not null)
            return false;

        var task = receiving.GetReceivingTask( taskId );
        
        return task is not null
            && task.InitializeReceiving( this, trailerId, dockId, areaId )
            && StartTask( task )
            && receiving.ActivateReceivingTask( task );
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
    public bool FinishReceiving( ReceivingOperations receiving )
    {
        return ReceivingTask is not null
            && EndTask()
            && receiving.CompleteTask( ReceivingTask );
    }
}