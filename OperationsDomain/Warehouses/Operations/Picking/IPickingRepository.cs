using OperationsDomain.Database;
using OperationsDomain.Warehouses.Operations.Picking.Models;

namespace OperationsDomain.Warehouses.Operations.Picking;

public interface IPickingRepository : IEfCoreRepository
{
    public Task<PickingOperations?> GetPickingOperationsWithTasks();
}