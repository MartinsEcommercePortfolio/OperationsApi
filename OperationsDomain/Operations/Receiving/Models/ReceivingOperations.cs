using OperationsDomain.Units;

namespace OperationsDomain.Operations.Receiving.Models;

public sealed class ReceivingOperations : Warehouse
{
    public Warehouse Warehouse { get; set; } = default!; 
    public List<ReceivingTask> PendingIntakeTasks { get; private set; } = [];
    public List<ReceivingTask> ActiveIntakeTasks { get; private set; } = [];

    public ReceivingTask? GetNextTask() =>
        PendingIntakeTasks.FirstOrDefault();

    public bool AddNewTask( Trailer trailer )
    {
        if (trailer.Dock is null)
            return false;
        
        var area = FindClosestArea( trailer.Dock );

        if (area is null)
            return false;

        var task = ReceivingTask.New( trailer, trailer.Dock, area );
        PendingIntakeTasks.Add( task );

        return trailer.AssignTask( task.Id );
    }
    public ReceivingTask? GetTask( Guid taskId ) =>
        PendingIntakeTasks.FirstOrDefault( t => t.Id == taskId );
    public bool ActivateTask( ReceivingTask receivingTask )
    {
        var accepted = receivingTask.IsStarted 
            && !receivingTask.IsFinished
            && !ActiveIntakeTasks.Contains( receivingTask ) 
            && PendingIntakeTasks.Remove( receivingTask );

        if (accepted)
            ActiveIntakeTasks.Add( receivingTask );

        return accepted;
    }
    public bool CompleteTask( ReceivingTask receivingTask )
    {
        return receivingTask.IsFinished
            && ActiveIntakeTasks.Remove( receivingTask );
    }

    Area? FindClosestArea( Dock dock )
    {
        return ReceivingAreas
            .Where( static a => !a.IsOwned() )
            .MinBy( b => Math.Abs( b.Number - dock.Number ) );
    }
}