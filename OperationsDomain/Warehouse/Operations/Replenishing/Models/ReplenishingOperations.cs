using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishingOperations
{
    public Guid Id { get; private set; }
    public List<ReplenishingTask> PendingReplenishingTasks { get; private set; } = [];
    public List<ReplenishingTask> ActiveReplenishingTasks { get; private set; } = [];
    
    public ReplenishingTask? GetNextReplenishingTask()
    {
        var task = PendingReplenishingTasks.FirstOrDefault();
        return task;
    }

    internal bool GenerateReplenishingTask( Pallet pallet, Racking from, Racking to )
    {
        var task = new ReplenishingTask( pallet, from, to );
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