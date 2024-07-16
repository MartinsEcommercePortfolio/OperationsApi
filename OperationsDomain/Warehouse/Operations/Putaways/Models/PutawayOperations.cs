using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Putaways.Models;

public sealed class PutawayOperations
{
    public List<Racking> Rackings { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<PutawayTask> PutawayTasks { get; private set; } = [];
    
    public async Task<PutawayTask?> StartPutawayTask( Employee employee, Guid palletId )
    {
        var pallet = Pallets.FirstOrDefault( p => p.Id == palletId );
        if (pallet is null)
            return null;

        var racking = await FindRackingForPutaway( pallet );
        if (racking is null)
            return null;

        PutawayTask putawayTask = new();

        var taskStarted = putawayTask.InitializePutaway( racking, pallet )
            && employee.StartTask( putawayTask )
            && employee.UnStagePallet( pallet.Area ?? new Area(), pallet );

        if (!taskStarted)
            return null;

        PutawayTasks.Add( putawayTask );
        return putawayTask;
    }
    public bool FinishPutawayTask( Employee employee, Guid rackingId, Guid palletId )
    {
        var task = employee.TaskAs<PutawayTask>();
        
        var completed = task.CompletePutaway( rackingId, palletId )
            && employee.EndTask()
            && PutawayTasks.Remove( task );

        return completed;
    }

    async Task<Racking?> FindRackingForPutaway( Pallet pallet )
    {
        return await Task.Run( () => {
            return Rackings
                .FirstOrDefault( static r => r.IsAvailable() );
        } );
    }
}