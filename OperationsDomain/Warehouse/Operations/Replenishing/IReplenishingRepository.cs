using OperationsDomain._Database;
using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Operations.Replenishing;

public interface IReplenishingRepository : IEfCoreRepository
{
    public Task<ReplenishingOperations?> GetReplenishingOperationsWithTasks();
}