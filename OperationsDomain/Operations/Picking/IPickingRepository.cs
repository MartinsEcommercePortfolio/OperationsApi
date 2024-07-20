using OperationsDomain._Database;
using OperationsDomain.Operations.Picking.Models;

namespace OperationsDomain.Operations.Picking;

public interface IPickingRepository : IEfCoreRepository
{
    public Task<PickingOperations?> GetPickingOperationsWithTasks();
}