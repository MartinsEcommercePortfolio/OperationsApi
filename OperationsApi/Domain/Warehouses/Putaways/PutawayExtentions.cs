using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses.Putaways;

internal static class PutawayExtentions
{
    public static async Task<Racking?> AssignPutaway( this Warehouse w, Employee employee, Pallet pallet )
    {
        if (pallet.IsOwned())
            return null;

        // Because there may be a lot of rackings to check
        return await Task.Run( () => {
            Racking? racking = w.Racks.FirstOrDefault(
                r => r.CanHoldPallet( pallet ) );

            if (racking is null)
                return null;

            racking.AssignTo( employee );
            pallet.AssignTo( employee );
            return racking;
        } );
    }
    public static bool ConfirmPutaway( this Warehouse w, Employee employee, Guid palletId, Guid rackingId )
    {
        var task = employee.GetTask<PutawayTask>();
        return task.Pallet.Id == palletId
            && task.ToRacking.Id == rackingId
            && task.Pallet.Place( task.ToRacking );
    }
}