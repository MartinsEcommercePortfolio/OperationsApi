using OperationsApi.Domain.Tasks;
using OperationsApi.Types.Tasks;

namespace OperationsApi.Features.WarehouseTasks.Picking;

internal sealed class PickingRepository
{
    public async Task<PickingTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}