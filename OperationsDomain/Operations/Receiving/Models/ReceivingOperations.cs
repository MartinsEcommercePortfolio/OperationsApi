using OperationsDomain.Units;

namespace OperationsDomain.Operations.Receiving.Models;

public sealed class ReceivingOperations
{
    public List<Area> Areas { get; private set; } = [];
    public List<ReceivingTask> Tasks { get; private set; } = [];
    public List<Guid> PendingTasks { get; private set; } = [];
    public List<Guid> ActiveTasks { get; private set; } = [];
    public List<Guid> CompletedTasks { get; private set; } = [];

    public List<ReceivingTask> GetCompletedTasks() =>
        Tasks.Where( t => CompletedTasks.Contains( t.Id ) ).ToList();
    public ReceivingTask? GetNextTask() =>
        Tasks.FirstOrDefault();
    public ReceivingTask? GetTask( Guid taskId ) =>
        Tasks.FirstOrDefault( t => t.Id == taskId );
    public bool AddNewTask( Trailer trailer )
    {
        if (trailer.Dock is null)
            return false;
        
        var area = FindClosestArea( trailer.Dock );

        if (area is null)
            return false;

        var task = ReceivingTask.New( trailer, trailer.Dock, area );
        Tasks.Add( task );
        PendingTasks.Add( task.Id );

        return trailer.AssignTask( task.Id );
    }
    public bool ActivateTask( ReceivingTask receivingTask )
    {
        var accepted = receivingTask.IsStarted 
            && !receivingTask.IsFinished
            && !ActiveTasks.Contains( receivingTask.Id )
            && !CompletedTasks.Contains( receivingTask.Id ) 
            && PendingTasks.Remove( receivingTask.Id );

        if (accepted)
            ActiveTasks.Add( receivingTask.Id );

        return accepted;
    }
    public bool CompleteTask( ReceivingTask receivingTask )
    {
        var completed = receivingTask.IsFinished
            && ActiveTasks.Remove( receivingTask.Id );

        if (completed)
            CompletedTasks.Add( receivingTask.Id );

        return completed;
    }
    public bool RemoveTask( ReceivingTask receivingTask )
    {
        return receivingTask.IsFinished
            && CompletedTasks.Remove( receivingTask.Id )
            && Tasks.Remove( receivingTask );
    }
    Area? FindClosestArea( Dock dock )
    {
        return Areas
            .Where( static a => !a.IsOwned() )
            .MinBy( b => Math.Abs( b.Number - dock.Number ) );
    }
}