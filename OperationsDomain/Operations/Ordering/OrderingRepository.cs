using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Operations.Ordering.Models;

namespace OperationsDomain.Operations.Ordering;

internal sealed class OrderingRepository( WarehouseDbContext dbContext, ILogger<OrderingRepository> logger ) 
    : DatabaseService<OrderingRepository>( dbContext, logger ), IOrderingRepository
{
    public async Task<OrderingOperations?> GetOrderingOperationsAll()
    {
        try
        {
            return await DbContext.Ordering
                .Include( static o => o.Products )
                .Include( static o => o.PendingOrders )
                .Include( static o => o.FulfillingOrders )
                .Include( static o => o.PickedOrders )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<OrderingOperations?>( e, null );
        }
    }
}