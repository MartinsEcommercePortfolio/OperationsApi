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

    internal bool ConfirmPickup( Guid rackingId, Guid palletId )
    {
        PalletHasBeenPicked = FromRacking.Id == rackingId
            && ReplenPallet.Id == palletId;
        
        return PalletHasBeenPicked;
    }
    internal bool ConfirmDeposit( Guid rackingId, Guid palletId )
    {
        IsFinished = PalletHasBeenPicked
            && ToRacking.Id == rackingId
            && ReplenPallet.Id == palletId
            && Employee.RackPallet( ToRacking, ReplenPallet );

        return IsFinished;
    }
}