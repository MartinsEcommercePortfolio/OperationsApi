using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Loading.Models;

namespace OperationsDomain.Domain.WarehouseSections.Loading;

public interface ILoadingRepository : IEfCoreRepository
{
    public Task<LoadingSection?> GetLoadingSectionWithTasks();
}