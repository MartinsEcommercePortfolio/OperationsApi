using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Outbound.Picking.Models;

namespace OperationsDomain.Outbound.Picking;

internal class PickingRepository( WarehouseDbContext dbContext, ILogger<PickingRepository> logger ) 
    : DatabaseService<PickingRepository>( dbContext, logger ), IPickingRepository
{
    public async Task<PickingOperations?> GetPickingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Picking
                .Include( static p => p.PendingPickingTasks )
                .Include( static p => p.ActivePickingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<PickingOperations?>( e, null );
        }
    }
}