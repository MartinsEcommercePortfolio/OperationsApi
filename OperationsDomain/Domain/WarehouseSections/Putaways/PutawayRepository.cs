using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Putaways;

internal sealed class PutawayRepository( WarehouseDbContext dbContext, ILogger<PutawayRepository> logger )
    : DatabaseService<PutawayRepository>( dbContext, logger ), IPutawayRepository
{
    readonly WarehouseDbContext _database = dbContext;
    
    public async Task<Racking?> StartPutawayTask( Employee employee, Guid palletId )
    {
        await using var transaction = await GetTransaction();
        try
        {
            var putaways = await _database.Putaways
                .Include( static p => p.Pallets )
                .Include( static p => p.Rackings )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
            
            if (putaways is null)
                return null;

            var assignedRacking = await putaways
                .BeginPutaway( employee, palletId )
                .ConfigureAwait( false );
            
            return assignedRacking is not null &&
                await SaveAsync( transaction )
                    ? assignedRacking
                    : null;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<Racking?>( e, transaction, null );
        }
    }
    public async Task<bool> CompletePutaway( Employee employee, Guid palletId, Guid rackingId )
    {
        await using var transaction = await GetTransaction();
        try
        {
            var putaways = await _database.Putaways
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
            
            return putaways is not null
                && putaways.CompletePutaway( employee, palletId, rackingId )
                && await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
}