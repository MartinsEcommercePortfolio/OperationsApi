using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Employees.Models.Variants;

public sealed class ReplenishingEmployee : Employee
{
    public ReplenishingTask? ReplenishingTask => TaskAs<ReplenishingTask>();

    public bool StartReplenishing( ReplenishingOperations replenishing, Guid taskId, Guid rackingId, Guid palletId )
    {
        if (ReplenishingTask is not null)
            return false;

        var task = replenishing.GetPendingTask( taskId );

        return task is not null
            && StartTask( task )
            && replenishing.ActivateTask( task )
            && task.ConfirmPickup( rackingId, palletId )
            && UnRackPallet( task.FromRacking, task.ReplenPallet );
    }
    public bool FinishReplenishing( ReplenishingOperations replenishing, Guid rackingId, Guid palletId )
    {
        return ReplenishingTask is not null
            && ReplenishingTask.ConfirmDeposit( rackingId, palletId )
            && RackPallet( ReplenishingTask.ToRacking, ReplenishingTask.ReplenPallet )
            && replenishing.RemovedCompletedTask( ReplenishingTask )
            && EndTask();
    }
}