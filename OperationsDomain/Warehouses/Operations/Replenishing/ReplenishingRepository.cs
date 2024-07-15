using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Warehouses.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouses.Operations.Replenishing;

internal sealed class ReplenishingRepository( WarehouseDbContext dbContext, ILogger<ReplenishingRepository> logger ) 
    : DatabaseService<ReplenishingRepository>( dbContext, logger ), IReplenishingRepository
{
    public async Task<ReplenishingOperations?> GetReplenishingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Replenishing
                .Include( static w => w.PendingReplenishingTasks )
                .Include( static w => w.ActiveReplenishingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReplenishingOperations?>( e, null );
        }
    }
}