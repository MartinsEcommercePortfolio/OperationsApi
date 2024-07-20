using OperationsDomain.Outbound.Shipping.Models;

namespace OperationsDomain.Outbound.Loading.Models;

public sealed class LoadingOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; } = []; 
    public List<LoadingTask> PendingLoadingTasks { get; private set; } = [];
    public List<LoadingTask> ActiveLoadingTasks { get; private set; } = [];
    public List<LoadingTask> CompletedLoadingTasks { get; private set; } = [];

    public LoadingTask? GetNextTask() =>
        PendingLoadingTasks.FirstOrDefault();
    public bool AddNewTask( LoadingTask task )
    {
        var added = PendingLoadingTasks.Contains( task )
            && !ActiveLoadingTasks.Contains( task )
            && !CompletedLoadingTasks.Contains( task );

        if (added)
            PendingLoadingTasks.Add( task );

        return added;
    }
    public bool RemoveTask( LoadingTask task )
    {
        return CompletedLoadingTasks.Remove( task );
    }
    
    internal LoadingTask? GetTask( Guid taskId ) =>
        PendingLoadingTasks.FirstOrDefault( t => t.Id == taskId );
    internal bool ActivateTask( LoadingTask task )
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
        var completed = task.IsFinished
            && ActiveLoadingTasks.Remove( task );
        
        if (completed)
            CompletedLoadingTasks.Add( task );

        return completed;
    }
}