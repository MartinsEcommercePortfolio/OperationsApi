using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Outbound.Loading.Models;

namespace OperationsDomain.Outbound.Loading;

internal sealed class LoadingRepository( WarehouseDbContext dbContext, ILogger<LoadingRepository> logger ) 
    : DatabaseService<LoadingRepository>( dbContext, logger ), ILoadingRepository
{
    public async Task<LoadingOperations?> GetLoadingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Loading
                .Include( static l => l.PendingLoadingTasks )
                .Include( static l => l.ActiveLoadingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<LoadingOperations?>( e, null );
        }
    }
}