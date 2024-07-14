using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Replenishing.Types;

namespace OperationsDomain.Domain.WarehouseSections.Replenishing;

internal sealed class ReplenishingRepository( WarehouseDbContext dbContext, ILogger<ReplenishingRepository> logger ) 
    : DatabaseService<ReplenishingRepository>( dbContext, logger ), IReplenishingRepository
{
    public async Task<ReplenishingSection?> GetReplenishingSectionWithTasks()
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
            return ProcessDbException<ReplenishingSection?>( e, null );
        }
    }
}