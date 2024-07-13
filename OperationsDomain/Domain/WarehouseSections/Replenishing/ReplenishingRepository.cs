using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Replenishing.Types;

namespace OperationsDomain.Domain.WarehouseSections.Replenishing;

internal sealed class ReplenishingRepository( WarehouseDbContext dbContext, ILogger<ReplenishingRepository> logger ) 
    : DatabaseService<ReplenishingRepository>( dbContext, logger ), IReplenishingRepository
{
    public async Task<ReplenishingTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
}