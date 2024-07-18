using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Ordering.Models;

namespace OperationsDomain.Ordering;

internal sealed class OrderingRepository( WarehouseDbContext dbContext, ILogger<OrderingRepository> logger ) 
    : DatabaseService<OrderingRepository>( dbContext, logger ), IOrderingRepository
{
    public async Task<OrderingOperations?> GetOrderingOperationsForNewOrder()
    {
        try
        {
            return await DbContext.Ordering
                .Include( static o => o.Products )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<OrderingOperations?>( e, null );
        }
    }
}