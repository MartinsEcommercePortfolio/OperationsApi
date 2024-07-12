using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses.Putaways;

internal static class PutawayExtentions
{
    public static async Task<Racking?> BeginPutaway( this Warehouse w, Employee employee, Pallet pallet )
    {
        if (!pallet.CanBePutAway())
            return null;
        
        return await Task.Run( () => {
            var racking = w.Racks.FirstOrDefault(
                r => r.CanTakePallet( pallet ) );
            
            if (racking is null)
                return null;

            var task = PutawayTask
                .StartNew( employee, pallet, racking );
            
            if (task is null)
                return null;

            w.ActivePutawayTasks.Add( task );
            return racking;
        } );
    }
    public static bool CompletePutaway( this Warehouse w, Employee employee, Guid palletId, Guid rackingId )
    {
        var task = employee
            .GetTask<PutawayTask>();
        var completed = task 
            .Complete( palletId, rackingId );

        if (completed)
            w.ActivePutawayTasks.Remove( task );

        return completed;
    }
}