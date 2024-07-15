using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishingTask : WarehouseTask
{
    public Pallet Pallet { get; set; } = default!;
    public Racking FromRacking { get; set; } = default!;
    public Racking ToRacking { get; set; } = default!;
    public Guid FromRackingId { get; set; }
    public Guid ToRackingId { get; set; }
    public bool PalletHasBeenPicked { get; set; }

    public bool PickupReplenishingPallet( Guid palletId )
    {
        PalletHasBeenPicked = Pallet.Id == palletId
            && Pallet.CanBePickedUp()
            && FromRacking.TakePallet( Pallet )
            && Pallet.GiveTo( Employee );
        return PalletHasBeenPicked;
    }
    public bool ReplenishLocation( Guid palletId, Guid rackingId )
    {
        bool replenished = PalletHasBeenPicked
            && Pallet.Id == palletId
            && ToRacking.Id == rackingId
            && ToRacking.TakePallet( Pallet )
            && Pallet.PutInRacking( ToRacking );

        if (replenished)
            IsCompleted = true;
        
        return replenished;
    }
}