using OperationsDomain.Units;

namespace OperationsDomain.Inbound.Putaways.Models;

public sealed class PutawayOperations
{
    public List<Racking> Rackings { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<PutawayTask> PutawayTasks { get; private set; } = [];

    internal async Task<PutawayTask?> GenerateTask( Guid palletId )
    {
        var pallet = FindPallet( palletId );
        if (pallet is null)
            return null;

        var racking = await FindRackingForPutaway( pallet );

        return racking is not null && pallet.Area is not null
            ? PutawayTask.New( pallet, pallet.Area, racking )
            : null;
    }
    internal bool ActivateTask( PutawayTask task )
    {
        var accepted = task.IsStarted
            && !PutawayTasks.Contains( task );
        
        if (accepted)
            PutawayTasks.Add( task );

        return accepted;
    }
    internal bool FinishPutawayTask( PutawayTask task )
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
                    r.Level != "0");
        } );
    }
}