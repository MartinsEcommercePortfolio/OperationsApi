using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Operations.Loading.Models;

namespace OperationsDomain.Operations.Loading;

internal sealed class LoadingRepository( WarehouseDbContext dbContext, ILogger<LoadingRepository> logger ) 
    : DatabaseService<LoadingRepository>( dbContext, logger ), ILoadingRepository
{
    public async Task<LoadingOperations?> GetLoadingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Loading
                .Include( static l => l.PendingTasks )
                .Include( static l => l.ActiveTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<LoadingOperations?>( e, null );
        }
    }
}