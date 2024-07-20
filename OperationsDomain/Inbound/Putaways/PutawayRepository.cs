using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Inbound.Putaways.Models;

namespace OperationsDomain.Inbound.Putaways;

internal sealed class PutawayRepository( WarehouseDbContext dbContext, ILogger<PutawayRepository> logger )
    : DatabaseService<PutawayRepository>( dbContext, logger ), IPutawayRepository
{
    public async Task<PutawayOperations?> GetPutawaysOperationsWithPalletsAndRackings()
    {
        try
        {
            return await DbContext.Putaways
                .Include( static p => p.Pallets )
                .Include( static p => p.Rackings )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<PutawayOperations?>( e, null );
        }
    }
    public async Task<PutawayOperations?> GetPutawaysOperationsWithTasks()
    {
        try
        {
            return await DbContext.Putaways
                .Include( static p => p.PutawayTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<PutawayOperations?>( e, null );
        }
    }
}