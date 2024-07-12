using OperationsApi.Types.Warehouses;

namespace OperationsApi.Domain.Tasks;

internal sealed class PickingTask : WarehouseTask
{
    public int CurrentPickIndex { get; set; }
    public Pallet? CurrentPickingFrom { get; set; }
    public List<Racking> PickLocations { get; set; } = [];
    public List<int> PickAmounts { get; set; } = [];
}