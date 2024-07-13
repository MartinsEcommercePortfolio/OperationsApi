using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Replenishing.Types;

namespace OperationsDomain.Domain.WarehouseSections.Replenishing;

public interface IReplenishingRepository : IEfCoreRepository
{
    public Task<ReplenishingTask?> GetNextTask( Guid employeeId );
}