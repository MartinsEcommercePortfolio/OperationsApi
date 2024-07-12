using OperationsApi.Domain.Warehouses.Picking;

namespace OperationsApi.Features.WarehouseTasks.Picking;

internal sealed class PickingRepository
{
    public async Task<PickingTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}