using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Replenishing.Models;

namespace OperationsDomain.Domain.WarehouseSections.Replenishing;

public interface IReplenishingRepository : IEfCoreRepository
{
    public Task<ReplenishingSection?> GetReplenishingSectionWithTasks();
}