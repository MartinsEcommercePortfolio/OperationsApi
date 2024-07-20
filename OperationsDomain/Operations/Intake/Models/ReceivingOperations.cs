using OperationsDomain.Operations.Shipping.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Operations.Intake.Models;

public sealed class ReceivingOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<ReceivingTask> PendingReceivingTasks { get; private set; } = [];
    public List<ReceivingTask> ActiveReceivingTasks { get; private set; } = [];

    public ReceivingTask? GetNextReceivingTask() =>
        PendingReceivingTasks.FirstOrDefault();

    public bool GenerateReceivingTask( Guid trailerId, Guid dockId, Guid areaId, List<Pallet> pallets )
    {
        var trailer = Trailers.FirstOrDefault( t => t.Id == trailerId );
        var dock = Docks.FirstOrDefault( d => d.Id == dockId );
        var area = Areas.FirstOrDefault( a => a.Id == areaId );

        var validTask = trailer is not null
            && dock is not null
            && area is not null
            && dock.Owner is null
            && dock.Trailer is null
            && area.Owner is null;

        if (!validTask)
            return false;
        
        Trailers.Add( trailer! );

        var task = ReceivingTask.New( trailer!, dock!, area! );
        PendingReceivingTasks.Add( task );

        return true;
    }
    public ReceivingTask? GetReceivingTask( Guid taskId ) =>
        PendingReceivingTasks.FirstOrDefault( t => t.Id == taskId );
    public bool ActivateReceivingTask( ReceivingTask receivingTask )
    {
        var accepted = receivingTask.IsStarted 
            && !receivingTask.IsFinished
            && !ActiveReceivingTasks.Contains( receivingTask ) 
            && PendingReceivingTasks.Remove( receivingTask );

        if (accepted)
            ActiveReceivingTasks.Add( receivingTask );

        return accepted;
    }
    public bool CompleteTask( ReceivingTask receivingTask )
    {
        return receivingTask.IsFinished
            && ActiveReceivingTasks.Remove( receivingTask );
    }
}