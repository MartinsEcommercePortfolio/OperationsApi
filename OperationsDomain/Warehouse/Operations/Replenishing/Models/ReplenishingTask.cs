using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishingTask : WarehouseTask
{
    public Pallet ReplenPallet { get; private set; } = default!;
    public Racking FromRacking { get; private set; } = default!;
    public Racking ToRacking { get; private set; } = default!;
    public Guid FromRackingId { get; private set; }
    public Guid ToRackingId { get; private set; }
    public bool PalletHasBeenPicked { get; private set; }

    public bool PickReplenishingPallet( Guid rackingId, Guid palletId )
    {
        PalletHasBeenPicked = FromRacking.Id == rackingId
            && ReplenPallet.Id == palletId
            && Employee.UnRackPallet( FromRacking, ReplenPallet );
        
        return PalletHasBeenPicked;
    }
    public bool ReplenishLocation( Guid rackingId, Guid palletId )
    {
        IsFinished = PalletHasBeenPicked
            && ToRacking.Id == rackingId
            && ReplenPallet.Id == palletId
            && Employee.RackPallet( ToRacking, ReplenPallet );

        return IsFinished;
    }
}