using OperationsApi.Domain.Tasks;
using OperationsApi.Types.Tasks;

namespace OperationsApi.Features.WarehouseTasks.Replenishing;

internal sealed class ReplenishingRepository
{
    public async Task<ReplenTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}