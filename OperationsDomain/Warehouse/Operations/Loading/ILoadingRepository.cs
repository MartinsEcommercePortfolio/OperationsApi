using OperationsDomain._Database;
using OperationsDomain.Warehouse.Operations.Loading.Models;

namespace OperationsDomain.Warehouse.Operations.Loading;

public interface ILoadingRepository : IEfCoreRepository
{
    public Task<LoadingOperations?> GetLoadingOperationsWithTasks();
}