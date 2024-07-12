using OperationsApi.Domain.WarehouseTasks;

namespace OperationsApi.Features.WarehouseTasks.Putaways;

internal sealed class PutawayRepository
{
    public async Task<PutawayTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}