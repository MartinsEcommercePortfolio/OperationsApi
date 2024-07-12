using OperationsApi.Domain.WarehouseTasks;

namespace OperationsApi.Features.WarehouseTasks.Replenishing;

internal sealed class ReplenishingRepository
{
    public async Task<ReplenishingTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}