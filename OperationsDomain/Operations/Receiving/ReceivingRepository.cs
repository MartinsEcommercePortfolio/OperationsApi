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
                .Include( static w => w.Tasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReceivingOperations?>( e, null );
        }
    }
}