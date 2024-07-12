using Microsoft.EntityFrameworkCore;
using OperationsApi.Database;
using OperationsApi.Domain.Employees;
using OperationsApi.Domain.Warehouses;
using OperationsApi.Domain.Warehouses.Putaways;
using OperationsApi.Features._Shared;

namespace OperationsApi.Features.WarehouseTasks.Putaways;

internal sealed class PutawayRepository( WarehouseDbContext dbContext, ILogger<PutawayRepository> logger )
    : DatabaseService<PutawayRepository>( dbContext, logger ), IEfCoreRepository
{
    readonly WarehouseDbContext _database = dbContext;
    
    public async Task<RackingDto?> StartPutaway( Employee employee, Guid palletId )
    {
        await using var transaction = await GetTransaction();
        try
        {
            var pallet = await _database.Pallets
                .FirstOrDefaultAsync( p => p.Id == palletId )
                .ConfigureAwait( false );
            
            if (pallet is null)
                return null;

            var warehouse = await _database.Warehouses
                .Include( static w => w.Racks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            if (warehouse is null)
                return null;

            var result = await warehouse
                .AssignPutaway( employee, pallet )
                .ConfigureAwait( false );
            
            return result is not null && await SaveAsync( transaction )
                ? RackingDto.FromModel( result )
                : null;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<RackingDto?>( e, transaction, null );
        }
    }
    public async Task<bool> ConfirmPutaway( Employee employee, Guid palletId, Racking Id )
    {
        await using var transaction = await GetTransaction();
        try
        {
            return false;
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
}