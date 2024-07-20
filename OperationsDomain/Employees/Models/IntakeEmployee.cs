using OperationsDomain.Operations.Intake.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Employees.Models;

public sealed class IntakeEmployee : Employee
{
    IntakeEmployee( Guid id, string name, Pallet? palletEquipped, IntakeTask? task )
        : base( id, name, palletEquipped, task ) { }
    
    public IntakeTask? ReceivingTask => 
        TaskAs<IntakeTask>();

    public bool StartIntake( IntakeOperations intake, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        if (Task is not null)
            return false;

        var task = intake.GetTask( taskId );
        
        return task is not null
            && StartTask( task )
            && task.VerifyStart( trailerId, dockId, areaId )
            && intake.ActivateTask( task );
    }
    public bool FinishIntake( IntakeOperations intake )
    {
        return ReceivingTask is not null
            && ReceivingTask.CleanUp( this )
            && EndTask()
            && intake.CompleteTask( ReceivingTask );
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