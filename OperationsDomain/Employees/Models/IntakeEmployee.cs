using OperationsDomain.Operations.Receiving.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Employees.Models;

public sealed class IntakeEmployee : Employee
{
    IntakeEmployee( Guid id, string name, Pallet? palletEquipped, ReceivingTask? task )
        : base( id, name, palletEquipped, task ) { }
    
    public ReceivingTask? ReceivingTask => 
        TaskAs<ReceivingTask>();

    public bool StartIntake( ReceivingOperations receiving, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        if (Task is not null)
            return false;

        var task = receiving.GetTask( taskId );
        
        return task is not null
            && StartTask( task )
            && task.VerifyStart( trailerId, dockId, areaId )
            && receiving.ActivateTask( task );
    }
    public bool FinishIntake( ReceivingOperations receiving )
    {
        return ReceivingTask is not null
            && ReceivingTask.CleanUp( this )
            && EndTask()
            && receiving.CompleteTask( ReceivingTask );
    }
    public bool UnloadPallet( Guid trailerId, Guid palletId )
    {
        var pallet = ReceivingTask?.StartUnloadingPallet( trailerId, palletId );

        return pallet is not null
            && ReceivingTask is not null
            && UnloadPallet( ReceivingTask.Trailer, pallet );
    }
    public bool StagePallet( Guid areaId, Guid palletId )
    {
        if (ReceivingTask is null)
            return false;
        
        return ReceivingTask?.CurrentPallet is not null
            && StagePallet( ReceivingTask.Area, ReceivingTask.CurrentPallet )
            && ReceivingTask.FinishUnloadingPallet( areaId, palletId );
    }
}