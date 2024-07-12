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
    
    public async Task<RackingDto?> BeginPutaway( Employee employee, Guid palletId )
    {
        await using var transaction = await GetTransaction();
        try
        {
            var warehouse = await _database.Warehouses
                .Include( static w => w.Pallets )
                .Include( static w => w.Racks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
            var pallet = warehouse?.GetPalletById( palletId );
            
            if (warehouse is null || pallet is null)
                return null;

            var assignedRacking = await warehouse
                .BeginPutaway( employee, pallet );
            
            return assignedRacking is not null &&
                await SaveAsync( transaction )
                    ? RackingDto.FromModel( assignedRacking )
                    : null;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<RackingDto?>( e, transaction, null );
        }
    }
    public async Task<bool> CompletePutaway( Employee employee, Guid palletId, Guid rackingId )
    {
        await using var transaction = await GetTransaction();
        try
        {
            var warehouse = await _database.Warehouses
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            if (warehouse is null)
                return false;

            return warehouse.CompletePutaway( employee, palletId, rackingId ) &&
                await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
}