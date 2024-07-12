using Microsoft.EntityFrameworkCore;
using OperationsApi.Database;
using OperationsApi.Domain.Warehouses;
using OperationsApi.Domain.WarehouseTasks.Receiving;
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
                .FirstOrDefaultAsync();
            
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
            ReceivingTask? task = await _database.PendingReceivingTasks
                .FirstOrDefaultAsync( t => t.Id == taskId )
                .ConfigureAwait( false );

            if (task is null)
                return false;
            
            task.Start( employee );
            _database.PendingReceivingTasks.Remove( task );

            await _database.ActiveReceivingTasks
                .AddAsync( task )
                .ConfigureAwait( false );

            return await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
    public async Task<bool> ReceiveTrailerPallet( Employee employee, Guid palletId )
    {
        await using var transaction = await GetTransaction();
        
        try
        {
            var task = employee.GetTask<ReceivingTask>();
            bool received =
                task.ReceiveTrailerPallet( employee, palletId ) &&
                await SaveAsync( transaction );
            return received;
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
    public async Task<bool> StageReceivedPallet( Employee employee, Guid palletId, Guid areaId )
    {
        await using var transaction = await GetTransaction();

        try
        {
            var task = employee.GetTask<ReceivingTask>();
            bool staged =
                task.StageReceivedPallet( employee, palletId, areaId ) &&
                await SaveAsync( transaction );
            return staged;
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
}