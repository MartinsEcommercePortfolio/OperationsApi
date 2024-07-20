using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Outbound.Shipping.Models;

namespace OperationsDomain.Outbound.Shipping;

internal sealed class ShippingRepository( WarehouseDbContext dbContext, ILogger<ShippingRepository> logger ) 
    : DatabaseService<ShippingRepository>( dbContext, logger ), IShippingRepository
{
    public async Task<ShippingOperations?> GetShippingOperationsWithRoutes()
    {
        try
        {
            return await DbContext.Shipping
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ShippingOperations?>( e, null );
        }
    }
}