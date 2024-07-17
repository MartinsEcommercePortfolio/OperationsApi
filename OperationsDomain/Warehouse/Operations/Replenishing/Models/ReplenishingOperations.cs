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

    internal bool SubmitReplenishEvent( Racking racking )
    {
        var existingPending = PendingReplenishingTasks
            .FirstOrDefault( p => p.ToRacking == racking );
        var existingActive = ActiveReplenishingTasks
            .FirstOrDefault( a => a.ToRacking == racking );
        
        if (existingPending is not null || existingActive is not null)
            return false;

        var fromRacking = Rackings.FirstOrDefault( r =>
            r.IsPickSlot &&
            r.Pallet is not null &&
            r.Product == racking.Product );

        return fromRacking is not null
            && GenerateReplenishingTask( racking );
    }
    internal bool GenerateReplenishingTask( Racking racking )
    {
        var existingTask = PendingReplenishingTasks.FirstOrDefault( r =>
            r.ToRacking == racking );

        if (existingTask is not null)
            return false;
        
        var fromRacking = Rackings.FirstOrDefault( r =>
            r.IsPickSlot &&
            r.Pallet is not null &&
            r.Product == racking.Product );

        if (fromRacking is null)
            return false;

        var task = new ReplenishingTask( fromRacking.Pallet!, fromRacking, racking );
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