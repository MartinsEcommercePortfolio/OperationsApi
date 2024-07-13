using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Putaways.Types;

public sealed class PutawaySection
{
    public List<Racking> Rackings { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<PutawayTask> PutawayTasks { get; set; } = [];
    
    public async Task<Racking?> BeginPutaway( Employee employee, Guid palletId )
    {
        Pallet? pallet = Pallets.FirstOrDefault( p => p.Id == palletId );
        if (pallet is null || !pallet.CanBePutAway())
            return null;
        
        return await Task.Run( () => {
            var racking = Rackings.FirstOrDefault(
                r => r.CanTakePallet( pallet ) );
            
            if (racking is null)
                return null;

            var task = PutawayTask
                .StartNew( employee, pallet, racking );
            
            if (task is null)
                return null;

            PutawayTasks.Add( task );
            return racking;
        } );
    }
    public bool CompletePutaway( Employee employee, Guid palletId, Guid rackingId )
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