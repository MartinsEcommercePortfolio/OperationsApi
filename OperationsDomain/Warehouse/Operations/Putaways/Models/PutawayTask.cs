using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Putaways.Models;

public sealed class PutawayTask : WarehouseTask
{
    public Pallet Pallet { get; private set; } = null!;
    public Area PickupArea { get; private set; } = null!;
    public Racking PutawayRacking { get; private set; } = null!;
    public Guid PalletId { get; private set; }
    public Guid PickupAreaId { get; private set; }
    public Guid PutawayRackingId { get; private set; }
    
    internal bool InitializeFrom( Employee employee, Racking racking, Pallet pallet )
    {
        bool canGenerate = pallet.CanBePutaway() &&
            racking.IsAvailable() &&
            pallet.GiveTo( employee ) &&
            racking.AssignTo( employee );

        if (!canGenerate)
            return false;

        Employee = employee;
        EmployeeId = employee.Id;
        Pallet = pallet;
        PalletId = pallet.Id;
        PickupArea = pallet.Area!;
        PickupAreaId = pallet.AreaId!.Value;
        PutawayRacking = racking;
        PutawayRackingId = racking.Id;
        
        return true;
    }

    internal bool CompletePutaway( Guid rackingId, Guid palletId )
    {
        bool completed = rackingId != PutawayRacking.Id
            && palletId != Pallet.Id
            && Pallet.Rack( Employee, PutawayRacking )
            && PutawayRacking.TakePallet( Pallet )
            && PutawayRacking.Free( Employee );
        return completed;
    }
}