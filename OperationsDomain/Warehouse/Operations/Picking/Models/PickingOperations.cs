using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; private set; }
    public List<PickingTask> PendingPickingTasks { get; private set; } = [];
    public List<PickingTask> ActivePickingTasks { get; private set; } = [];
    public List<ReplenishEvent> ReplenishEvents { get; private set; } = [];
    
    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    internal void SubmitReplenishEvent( ReplenishEvent e )
    {
        ReplenishEvents.Add( e );
    }
    internal PickingTask? GetPendingTask( Guid taskId ) =>
        PendingPickingTasks.FirstOrDefault( t => t.Id == taskId );
    internal bool ActivateTask( PickingTask task )
    {
        var accepted = task.IsStarted
            && !task.IsFinished
            && !ActivePickingTasks.Contains( task )
            && PendingPickingTasks.Remove( task );
        
        if (accepted)
            ActivePickingTasks.Add( task );

        return accepted;
    }
    internal bool RemoveCompletedTask( PickingTask task )
    {
        return task.IsFinished
            && ActivePickingTasks.Remove( task );
    }
}