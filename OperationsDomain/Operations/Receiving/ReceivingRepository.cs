using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Operations.Receiving.Models;

namespace OperationsDomain.Operations.Receiving;

internal sealed class ReceivingRepository( WarehouseDbContext dbContext, ILogger<ReceivingRepository> logger ) 
    : DatabaseService<ReceivingRepository>( dbContext, logger ), IReceivingRepository
{
    public async Task<ReceivingOperations?> GetReceivingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Intake
                .Include( static w => w.PendingIntakeTasks )
                .Include( static w => w.ActiveIntakeTasks )
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
            return await DbContext.Intake
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