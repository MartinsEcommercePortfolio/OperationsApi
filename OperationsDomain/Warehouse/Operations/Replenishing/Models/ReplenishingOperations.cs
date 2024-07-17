using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishingOperations
{
    public Guid Id { get; private set; }
    public List<Racking> Rackings { get; private set; } = [];
    public List<ReplenishEvent> ReplenishEvents { get; private set; } = [];
    public List<ReplenishingTask> PendingReplenishingTasks { get; private set; } = [];
    public List<ReplenishingTask> ActiveReplenishingTasks { get; private set; } = [];
    
    public ReplenishingTask? GetNextReplenishingTask()
    {
        var task = PendingReplenishingTasks.FirstOrDefault();
        return task;
    }

    internal bool SubmitReplenishEvent( ReplenishEvent e )
    {
        var existingPending = PendingReplenishingTasks
            .FirstOrDefault( p => p.ToRacking == e.Racking );
        var existingActive = ActiveReplenishingTasks
            .FirstOrDefault( a => a.ToRacking == e.Racking );
        
        if (existingPending is not null || existingActive is not null)
            return false;

        var fromRacking = Rackings.FirstOrDefault( r =>
            r.IsPickSlot &&
            r.Pallet is not null &&
            r.Product == e.Racking.Product );

        if (fromRacking is null)
            return false;

        ReplenishEvents.Add( e );
        return true;
    }
    internal bool GenerateReplenishingTask( ReplenishEvent replenishEvent )
    {
        var existingTask = PendingReplenishingTasks.FirstOrDefault( r =>
            r.ToRacking == replenishEvent.Racking );

        if (existingTask is not null)
            return false;
        
        var fromRacking = Rackings.FirstOrDefault( r =>
            r.IsPickSlot &&
            r.Pallet is not null &&
            r.Product == replenishEvent.Racking.Product );

        if (fromRacking is null)
            return false;

        var task = new ReplenishingTask( fromRacking.Pallet!, fromRacking, replenishEvent.Racking );
        PendingReplenishingTasks.Add( task );
        return true;
    }
    internal ReplenishingTask? GetPendingTask( Guid taskId ) =>
        PendingReplenishingTasks.FirstOrDefault( t => t.Id == taskId );
    internal bool ActivateTask( ReplenishingTask task )
    {
        var accepted = task.IsStarted
            && !task.IsFinished
            && !ActiveReplenishingTasks.Contains( task )
            && PendingReplenishingTasks.Remove( task );

        if (accepted)
            ActiveReplenishingTasks.Add( task );

        return accepted;
    }
    internal bool RemovedCompletedTask( ReplenishingTask task )
    {
        return task.IsFinished
            && ActiveReplenishingTasks.Remove( task );
    }
}