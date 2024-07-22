using OperationsDomain.Units;

namespace OperationsDomain.Operations.Loading.Models;

public sealed class LoadingOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; } = [];
    public List<LoadingTask> Tasks { get; private set; } = [];
    public List<Guid> PendingTasks { get; private set; } = [];
    public List<Guid> ActiveTasks { get; private set; } = [];
    public List<Guid> CompletedTasks { get; private set; } = [];

    public List<LoadingTask> GetCompletedTasks() =>
        Tasks.Where( t => CompletedTasks.Contains( t.Id ) ).ToList();
    public LoadingTask? GetNextTask() =>
        Tasks.FirstOrDefault( t => PendingTasks.Contains( t.Id ) );
    public bool AddNewTask( LoadingTask task )
    {
        var added = !Tasks.Contains( task )
            && !PendingTasks.Contains( task.Id )
            && !ActiveTasks.Contains( task.Id )
            && !CompletedTasks.Contains( task.Id );

        if (added)
            return false;
        
        Tasks.Add( task );
        PendingTasks.Add( task.Id );

        return true;
    }
    public bool RemoveTask( LoadingTask task )
    {
        return CompletedTasks.Remove( task.Id )
            && Tasks.Remove( task );
    }
    
    internal LoadingTask? GetTask( Guid taskId ) =>
        Tasks.FirstOrDefault( t => t.Id == taskId );
    internal bool ActivateTask( LoadingTask task )
    {
        var accepted = task.IsStarted
            && !task.IsFinished
            && !ActiveTasks.Contains( task.Id )
            && PendingTasks.Remove( task.Id );
        
        if (accepted)
            ActiveTasks.Add( task.Id );

        return accepted;
    }
    internal bool CompleteTask( LoadingTask task )
    {
        var completed = task.IsFinished
            && ActiveTasks.Remove( task.Id );
        
        if (completed)
            CompletedTasks.Add( task.Id );

        return completed;
    }
}