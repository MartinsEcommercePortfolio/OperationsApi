using OperationsDomain._Database;
using OperationsDomain.Outbound.Loading.Models;

namespace OperationsDomain.Outbound.Loading;

public interface ILoadingRepository : IEfCoreRepository
{
    public Task<LoadingOperations?> GetLoadingOperationsWithTasks();
}