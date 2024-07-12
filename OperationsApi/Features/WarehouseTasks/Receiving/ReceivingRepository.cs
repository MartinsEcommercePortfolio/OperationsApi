using Microsoft.EntityFrameworkCore;
using OperationsApi.Database;
using OperationsApi.Domain.Employees;
using OperationsApi.Domain.Warehouses.Receiving;
using OperationsApi.Features.WarehouseTasks.Receiving.Dtos;

namespace OperationsApi.Features.WarehouseTasks.Receiving;

internal sealed class ReceivingRepository( WarehouseDbContext dbContext, ILogger<ReceivingRepository> logger ) 
    : DatabaseService<ReceivingRepository>( dbContext, logger ), IEfCoreRepository
{
    readonly WarehouseDbContext _database = dbContext;

    public async Task<ReceivingTaskSummary?> GetNextReceivingTask()
    {
        try
        {
            var task = await _database.PendingReceivingTasks
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
            
            return task is not null
                ? ReceivingTaskSummary.FromModel( task )
                : null;
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReceivingTaskSummary?>( e, null );
        }
    }
    public async Task<bool> StartReceiving( Employee employee, Guid taskId )
    {
        await using var transaction = await GetTransaction();
        
        try
        {
            var warehouse = await _database.Warehouses
                .Include( static w => w.PendingReceivingTasks )
                .Include( static w => w.ActiveReceivingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            return warehouse is not null
                && warehouse.StartReceivingTask( employee, taskId )
                && await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
    public async Task<bool> ReceivePallet( Employee employee, Guid palletId )
    {
        await using var transaction = await GetTransaction();
        
        try
        {
            var warehouse = await _database.Warehouses
                .Include( static w => w.Pallets )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            return warehouse is not null
                && warehouse.ReceivePallet( employee, palletId )
                && await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
    public async Task<bool> StagePallet( Employee employee, Guid palletId, Guid areaId )
    {
        await using var transaction = await GetTransaction();
        
        try
        {
            var warehouse = await _database.Warehouses
                .Include( static w => w.Pallets )
                .Include( static w => w.Areas )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            return warehouse is not null
                && warehouse.StagePallet( employee, palletId, areaId )
                && await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
}