
using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses.Picking;

internal sealed class PickingTask : WarehouseTask
{
    public PickLine? CurrentPickLine { get; set; }
    public List<PickLine> PickLines { get; set; } = [];

    public bool PickItem( Employee employee, Guid itemId, Guid rackingId )
    {
        return false;
    }
}