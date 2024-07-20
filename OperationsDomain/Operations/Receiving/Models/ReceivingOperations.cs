using OperationsDomain.Units;

namespace OperationsDomain.Operations.Receiving.Models;

public sealed class ReceivingOperations : Warehouse
{
    public Warehouse Warehouse { get; set; } = default!; 
    public List<ReceivingTask> PendingIntakeTasks { get; private set; } = [];
    public List<ReceivingTask> ActiveIntakeTasks { get; private set; } = [];

    public ReceivingTask? GetNextTask() =>
        PendingIntakeTasks.FirstOrDefault();

    public bool GenerateTask( Guid trailerId, Guid dockId, Guid areaId, List<Pallet> pallets )
    {
        var trailer = Warehouse.ReceivingTrailers.FirstOrDefault( t => t.Id == trailerId );
        var dock = Warehouse.ReceivingDocks.FirstOrDefault( d => d.Id == dockId );
        var area = Warehouse.ReceivingAreas.FirstOrDefault( a => a.Id == areaId );

        var validTask = trailer is not null
            && dock is not null
            && area is not null
            && dock.Owner is null
            && dock.Trailer is null
            && area.Owner is null;

        if (!validTask)
            return false;

        Warehouse.ReceivingTrailers.Add( trailer! );

        var task = ReceivingTask.New( trailer!, dock!, area! );
        PendingIntakeTasks.Add( task );

        return true;
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
}