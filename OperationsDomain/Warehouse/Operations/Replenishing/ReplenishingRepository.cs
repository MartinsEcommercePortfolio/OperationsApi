using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Operations.Replenishing;

internal sealed class ReplenishingRepository( WarehouseDbContext dbContext, ILogger<ReplenishingRepository> logger ) 
    : DatabaseService<ReplenishingRepository>( dbContext, logger ), IReplenishingRepository
{
    public async Task<ReplenishingOperations?> GetReplenishingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Replenishing
                .Include( static r => r.PendingReplenishingTasks )
                .Include( static r => r.ActiveReplenishingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReplenishingOperations?>( e, null );
        }
    }
    public async Task<ReplenishingOperations?> GetReplenishingOperationsWithEventsAndTasks()
    {
        try
        {
            return await DbContext.Replenishing
                .Include( static r => r.ReplenishEvents )
                .Include( static r => r.PendingReplenishingTasks )
                .Include( static r => r.ActiveReplenishingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReplenishingOperations?>( e, null );
        }
    }
}