using OperationsDomain._Database;
using OperationsDomain.Operations.Loading.Models;

namespace OperationsDomain.Operations.Loading;

public interface ILoadingRepository : IEfCoreRepository
{
    public Task<LoadingOperations?> GetLoadingOperationsWithTasks();
}