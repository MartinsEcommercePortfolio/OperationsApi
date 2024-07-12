using OperationsApi.Domain.Tasks;
using OperationsApi.Types.Tasks;

namespace OperationsApi.Features.WarehouseTasks.Putaways;

internal sealed class PutawayRepository
{
    public async Task<PutawayTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}