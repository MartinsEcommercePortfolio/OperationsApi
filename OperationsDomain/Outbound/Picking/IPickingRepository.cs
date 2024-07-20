using OperationsDomain._Database;
using OperationsDomain.Outbound.Picking.Models;

namespace OperationsDomain.Outbound.Picking;

public interface IPickingRepository : IEfCoreRepository
{
    public Task<PickingOperations?> GetPickingOperationsWithTasks();
}