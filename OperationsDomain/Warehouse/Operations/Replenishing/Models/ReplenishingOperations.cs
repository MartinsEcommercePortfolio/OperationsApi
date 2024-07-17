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
    internal ReplenishingTask? GetReplenishingTask( Guid taskId ) =>
        PendingReplenishingTasks.FirstOrDefault( t => t.Id == taskId );
    internal bool AcceptReplenishingTask( ReplenishingTask task )
    {
        var accepted = task.IsStarted
            && !task.IsFinished
            && !ActiveReplenishingTasks.Contains( task )
            && PendingReplenishingTasks.Remove( task );

        if (accepted)
            ActiveReplenishingTasks.Add( task );

        return accepted;
    }
    internal bool FinishReplenishingTask( ReplenishingTask task )
    {
        return task.IsFinished
            && ActiveReplenishingTasks.Remove( task );
    }
}