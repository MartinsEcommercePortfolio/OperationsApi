using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Putaways.Models;

public sealed class PutawayOperations
{
    public List<Racking> Rackings { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<PutawayTask> PutawayTasks { get; private set; } = [];
    
    internal bool AcceptPutawayTask( PutawayTask task )
    {
        var accepted = task.IsStarted
            && !PutawayTasks.Contains( task );
        
        if (accepted)
            PutawayTasks.Add( task );

        return accepted;
    }
    public bool FinishPutawayTask( PutawayTask task )
    {
        return task.IsFinished
            && PutawayTasks.Remove( task );
    }
        
    internal Pallet? FindPallet( Guid palletId )
    {
        return Pallets.FirstOrDefault( p => p.Id == palletId );
    }
    internal async Task<Racking?> FindRackingForPutaway( Pallet pallet )
    {
        return await Task.Run( () => {
            return Rackings
                .FirstOrDefault( r => 
                    r.IsAvailable() &&
                    r.Level != "0" &&
                    r.PalletFits( pallet ) );
        } );
    }
}