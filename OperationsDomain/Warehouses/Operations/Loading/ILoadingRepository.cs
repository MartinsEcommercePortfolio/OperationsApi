using OperationsDomain.Database;
using OperationsDomain.Warehouses.Operations.Loading.Models;

namespace OperationsDomain.Warehouses.Operations.Loading;

public interface ILoadingRepository : IEfCoreRepository
{
    public Task<LoadingOperations?> GetLoadingOperationsWithTasks();
}