using OperationsDomain.Database;
using OperationsDomain.Warehouses.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouses.Operations.Replenishing;

public interface IReplenishingRepository : IEfCoreRepository
{
    public Task<ReplenishingOperations?> GetReplenishingOperationsWithTasks();
}