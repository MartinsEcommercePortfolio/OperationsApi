using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Loading.Models;

namespace OperationsDomain.Domain.WarehouseSections.Loading;

internal sealed class LoadingRepository( WarehouseDbContext dbContext, ILogger<LoadingRepository> logger ) 
    : DatabaseService<LoadingRepository>( dbContext, logger ), ILoadingRepository
{
    public async Task<LoadingSection?> GetLoadingSectionWithTasks()
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
            return ProcessDbException<LoadingSection?>( e, null );
        }
    }
}