using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Replenishing.Types;

public sealed class ReplenishingTask : WarehouseTask
{
    public Pallet Pallet { get; set; } = null!;
    public Racking FromRacking { get; set; } = null!;
    public Racking ToRacking { get; set; } = null!;
    public Guid FromRackingId { get; set; }
    public Guid ToRackingId { get; set; }
    public bool PalletHasBeenPicked { get; set; }
}