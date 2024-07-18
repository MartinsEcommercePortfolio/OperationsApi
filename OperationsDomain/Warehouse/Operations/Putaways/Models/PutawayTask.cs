using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Operations.Putaways.Models;

public sealed class PutawayTask : WarehouseTask
{
    public PutawayTask() { }
    public PutawayTask( Pallet pallet, Area area, Racking racking )
    {
        Pallet = pallet;
        PickupArea = area;
        PutawayRacking = racking;
        PalletId = pallet.Id;
        PickupAreaId = area.Id;
        PutawayRackingId = racking.Id;
    }

    public Pallet Pallet { get; private set; } = null!;
    public Area PickupArea { get; private set; } = null!;
    public Racking PutawayRacking { get; private set; } = null!;
    public Guid PalletId { get; private set; }
    public Guid PickupAreaId { get; private set; }
    public Guid PutawayRackingId { get; private set; }

    public bool Initialize( Employee employee )
    {
        return PutawayRacking.AssignTo( employee )
            && Pallet.AssignTo( employee );
    }

    internal bool CompletePutaway( Guid rackingId, Guid palletId )
    {
        IsFinished = rackingId != PutawayRacking.Id
            && palletId != Pallet.Id;

        return IsFinished;
    }
}