
using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses.Putaways;

internal sealed class PutawayTask : WarehouseTask
{
    public Pallet Pallet { get; set; } = null!;
    public Area PickupArea { get; set; } = null!;
    public Racking PutawayRacking { get; set; } = null!;
    public Guid PalletId { get; set; }
    public Guid PickupAreaId { get; set; }
    public Guid PutawayRackingId { get; set; }

    public static PutawayTask? StartNew( Employee employee, Pallet pallet, Racking racking )
    {
        var task = new PutawayTask {
            Employee = employee,
            EmployeeId = employee.Id,
            Pallet = pallet,
            PalletId = pallet.Id,
            PickupArea = pallet.Area!,
            PickupAreaId = pallet.AreaId,
            PutawayRacking = racking,
            PutawayRackingId = racking.Id
        };

        pallet.AssignTo( employee );
        racking.AssignTo( employee );
        return employee.StartTask( task )
            ? task
            : null;
    }

    public bool Complete( Guid palletId, Guid rackingId )
    {
        if (palletId != Pallet.Id || rackingId != PutawayRacking.Id)
            return false;
        
        Pallet.PutAway( PutawayRacking );
        Employee.FinishTask();
        return true;
    }
}