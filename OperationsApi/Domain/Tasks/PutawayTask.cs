using OperationsApi.Types.Warehouses;

namespace OperationsApi.Domain.Tasks;

internal sealed class PutawayTask : WarehouseTask
{
    public Pallet Pallet { get; set; } = null!;
    public Area FromArea { get; set; } = null!;
    public Racking ToRacking { get; set; } = null!;
    public Guid PalletId { get; set; }
    public Guid FromAreaId { get; set; }
    public Guid ToRackingId { get; set; }
    public bool PalletHasBeenPicked { get; set; }
}