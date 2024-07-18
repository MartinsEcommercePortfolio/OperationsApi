namespace OperationsDomain.Warehouse.Operations.Loading.Models;

public sealed class LoadingOperations
{
    public Guid Id { get; private set; }
    public List<LoadingTask> PendingLoadingTasks { get; private set; } = [];
    public List<LoadingTask> ActiveLoadingTasks { get; private set; } = [];

    public LoadingTask? GetNextTask() =>
        PendingLoadingTasks.FirstOrDefault();
    
    internal LoadingTask? GetTask( Guid taskId ) =>
        PendingLoadingTasks.FirstOrDefault( t => t.Id == taskId );
    internal bool AcceptTask( LoadingTask task )
    {
        var accepted = task.IsStarted
            && !task.IsFinished
            && !ActiveLoadingTasks.Contains( task )
            && PendingLoadingTasks.Remove( task );
        
        if (accepted)
            ActiveLoadingTasks.Add( task );

        return accepted;
    }
    internal bool CompleteTask( LoadingTask task )
    {
        return task.IsFinished
            && ActiveLoadingTasks.Remove( task );
    }
}