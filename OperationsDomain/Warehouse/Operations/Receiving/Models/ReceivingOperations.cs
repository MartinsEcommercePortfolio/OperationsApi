using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Receiving.Models;

public sealed class ReceivingOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<ReceivingTask> PendingReceivingTasks { get; private set; } = [];
    public List<ReceivingTask> ActiveReceivingTasks { get; private set; } = [];

    public ReceivingTask? GetNextReceivingTask() =>
        PendingReceivingTasks.FirstOrDefault();
    
    internal bool ReceiveTrailerWithPallets( string trailerNumber, string dockNumber, List<Pallet> pallets )
    {
        return false;
    }
    internal ReceivingTask? GetReceivingTask( Guid taskId ) =>
        PendingReceivingTasks.FirstOrDefault( t => t.Id == taskId );
    internal bool AcceptReceivingTask( ReceivingTask receivingTask )
    {
        var accepted = receivingTask.IsStarted 
            && !receivingTask.IsFinished
            && !ActiveReceivingTasks.Contains( receivingTask ) 
            && PendingReceivingTasks.Remove( receivingTask );

        if (accepted)
            ActiveReceivingTasks.Add( receivingTask );

        return accepted;
    }
    internal bool CompleteTask( ReceivingTask receivingTask )
    {
        return receivingTask.IsFinished
            && ActiveReceivingTasks.Remove( receivingTask );
    }
}