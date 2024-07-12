using OperationsApi.Domain.Warehouses.Replenishing;

namespace OperationsApi.Features.WarehouseTasks.Replenishing;

internal sealed class ReplenishingRepository
{
    public async Task<ReplenishingTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}