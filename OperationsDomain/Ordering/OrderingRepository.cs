using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Ordering.Models;

namespace OperationsDomain.Ordering;

internal sealed class OrderingRepository( WarehouseDbContext dbContext, ILogger<OrderingRepository> logger ) 
    : DatabaseService<OrderingRepository>( dbContext, logger ), IOrderingRepository
{
    public async Task<OrderingOperations?> GetOrderingOperationsAll()
    {
        try
        {
            return await DbContext.Ordering
                .Include( static o => o.PendingOrders )
                .Include( static o => o.PickingOrders )
                .Include( static o => o.LoadingOrders )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<OrderingOperations?>( e, null );
        }
    }
}