using OperationsDomain._Database;
using OperationsDomain.Warehouse.Operations.Picking.Models;

namespace OperationsDomain.Warehouse.Operations.Picking;

public interface IPickingRepository : IEfCoreRepository
{
    public Task<PickingOperations?> GetPickingOperationsWithTasks();
    public Task<PickingOperations?> GetPickingOperationsWithEvents();
}