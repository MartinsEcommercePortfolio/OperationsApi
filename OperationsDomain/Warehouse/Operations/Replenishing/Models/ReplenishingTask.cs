using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishingTask : WarehouseTask
{
    public Pallet Pallet { get; private set; } = default!;
    public Racking FromRacking { get; private set; } = default!;
    public Racking ToRacking { get; private set; } = default!;
    public Guid FromRackingId { get; private set; }
    public Guid ToRackingId { get; private set; }
    public bool PalletHasBeenPicked { get; private set; }

    internal bool PickupReplenishingPallet( Guid palletId )
    {
        PalletHasBeenPicked = Pallet.Id == palletId
            && Pallet.CanBePicked()
            && FromRacking.TakePallet( Pallet )
            && Pallet.AssignTo( Employee );
        
        return PalletHasBeenPicked;
    }
    internal bool ReplenishLocation( Guid rackingId, Guid palletId )
    {
        IsFinished = PalletHasBeenPicked
            && ToRacking.Id == rackingId
            && Pallet.Id == palletId
            && ToRacking.TakePallet( Pallet )
            && Pallet.Rack( Employee, ToRacking );

        return IsFinished;
    }
}