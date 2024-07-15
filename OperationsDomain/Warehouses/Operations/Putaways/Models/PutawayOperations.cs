using OperationsDomain.Warehouses.Employees;
using OperationsDomain.Warehouses.Infrastructure;

namespace OperationsDomain.Warehouses.Operations.Putaways.Models;

public sealed class PutawayOperations
{
    public List<Racking> Rackings { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<PutawayTask> PutawayTasks { get; set; } = [];
    
    public async Task<PutawayTask?> BeginPutaway( Employee employee, Guid palletId )
    {
        Pallet? pallet = Pallets.FirstOrDefault( p => p.Id == palletId );
        if (pallet is null || !pallet.CanBePutAway())
            return null;
        
        return await Task.Run( () => {
            var racking = Rackings.FirstOrDefault(
                r => r.TakePallet( pallet ) );
            
            if (racking is null)
                return null;

            var task = PutawayTask
                .Initialize( employee, pallet, racking );
            
            if (task is null)
                return null;

            PutawayTasks.Add( task );
            return task;
        } );
    }
    public bool FinishPutaway( Employee employee, Guid palletId, Guid rackingId )
    {
        var task = employee
            .GetTask<PutawayTask>();
        var completed = task 
            .Complete( palletId, rackingId );

        if (completed)
            PutawayTasks.Remove( task );

        return completed;
    }
}