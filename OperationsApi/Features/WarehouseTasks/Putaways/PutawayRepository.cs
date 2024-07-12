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
    
    public async Task<RackingDto?> AssignPutaway( Guid palletId, Employee employee )
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
            if (pallet is null)
                return null;

            var racking = await warehouse!.AssignPutaway( employee, pallet );
            return racking is not null
                ? RackingDto.FromModel( racking )
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