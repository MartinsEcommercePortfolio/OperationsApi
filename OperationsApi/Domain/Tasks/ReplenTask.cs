using OperationsApi.Types.Warehouses;

namespace OperationsApi.Domain.Tasks;

internal sealed class ReplenTask : WarehouseTask
{
    public Pallet Pallet { get; set; } = null!;
    public Racking FromRacking { get; set; } = null!;
    public Racking ToRacking { get; set; } = null!;
    public Guid FromRackingId { get; set; }
    public Guid ToRackingId { get; set; }
    public bool PalletHasBeenPicked { get; set; }
}