using OperationsDomain.Warehouse.Operations.Putaways.Models;

namespace OperationsDomain.Warehouse.Employees.Models.Variants;

public sealed class PutawayEmployee : Employee
{
    public PutawayTask? PutawayTask => TaskAs<PutawayTask>();
    
    public async Task<bool> StartPutaway( PutawayOperations putaways, Guid palletId )
    {
        if (PutawayTask is not null)
            return false;

        var putawayTask = await putaways.GenerateTask( palletId );

        return putawayTask is not null
            && StartTask( putawayTask )
            && putaways.AcceptPutawayTask( putawayTask )
            && UnStagePallet( putawayTask.PickupArea, putawayTask.Pallet );
    }
    public bool FinishPutaway( PutawayOperations putaways, Guid rackingId, Guid palletId )
    {
        return PutawayTask is not null
            && PutawayTask.CompletePutaway( rackingId, palletId )
            && RackPallet( PutawayTask.PutawayRacking, PutawayTask.Pallet )
            && putaways.FinishPutawayTask( PutawayTask );
    }
}