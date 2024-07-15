using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsDomain.Warehouse.Operations.Receiving;

internal sealed class ReceivingRepository( WarehouseDbContext dbContext, ILogger<ReceivingRepository> logger ) 
    : DatabaseService<ReceivingRepository>( dbContext, logger ), IReceivingRepository
{
    public async Task<ReceivingOperations?> GetReceivingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Receiving
                .Include( static w => w.PendingReceivingTasks )
                .Include( static w => w.ActiveReceivingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReceivingOperations?>( e, null );
        }
    }
    public async Task<ReceivingOperations?> GetReceivingSectionOperationsPallets()
    {
        try
        {
            return await DbContext.Receiving
                .Include( static r => r.Pallets )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReceivingOperations?>( e, null );
        }
    }
}