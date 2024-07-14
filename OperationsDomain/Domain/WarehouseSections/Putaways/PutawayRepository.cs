using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Putaways.Types;

namespace OperationsDomain.Domain.WarehouseSections.Putaways;

internal sealed class PutawayRepository( WarehouseDbContext dbContext, ILogger<PutawayRepository> logger )
    : DatabaseService<PutawayRepository>( dbContext, logger ), IPutawayRepository
{
    public async Task<PutawaySection?> GetPutawaysSectionWithPalletsAndRackings()
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
            return ProcessDbException<PutawaySection?>( e, null );
        }
    }
    public async Task<PutawaySection?> GetPutawaysSectionWithTasks()
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
            return ProcessDbException<PutawaySection?>( e, null );
        }
    }
}