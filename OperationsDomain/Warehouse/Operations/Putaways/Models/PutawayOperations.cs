using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Putaways.Models;

public sealed class PutawayOperations
{
    public List<Racking> Rackings { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<PutawayTask> PutawayTasks { get; private set; } = [];
    
    public async Task<PutawayTask?> StartPutaway( Employee employee, Guid palletId )
    {
        var pallet = Pallets.FirstOrDefault( p => p.Id == palletId );
        if (pallet is null)
            return null;
        
        return await Task.Run( () => {
            var racking = Rackings
                .FirstOrDefault( static r => r.IsAvailable());
            
            if (racking is null)
                return null;

            PutawayTask task = new();
            
            var taskStarted = employee
                .StartPutawayTask( task, racking, pallet );

            if (!taskStarted)
                return null;
            
            PutawayTasks.Add( task );
            return task;
        } );
    }
    public bool FinishPutaway( Employee employee, Guid rackingId, Guid palletId )
    {
        var task = employee.TaskAs<PutawayTask>();
        
        var completed = employee.FinishPutaway( rackingId, palletId )
            && PutawayTasks.Remove( task );

        return completed;
    }
}