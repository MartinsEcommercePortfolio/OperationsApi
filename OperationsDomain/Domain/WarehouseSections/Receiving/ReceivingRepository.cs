using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Receiving.Types;

namespace OperationsDomain.Domain.WarehouseSections.Receiving;

internal sealed class ReceivingRepository( WarehouseDbContext dbContext, ILogger<ReceivingRepository> logger ) 
    : DatabaseService<ReceivingRepository>( dbContext, logger ), IReceivingRepository
{
    readonly WarehouseDbContext _database = dbContext;

    public async Task<ReceivingTask?> GetNextReceivingTask()
    {
        try
        {
            return await _database.PendingReceivingTasks
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReceivingTask?>( e, null );
        }
    }
    public async Task<bool> StartReceivingTask( Employee employee, Guid taskId )
    {
        await using var transaction = await GetTransaction();
        
        try
        {
            var receiving = await _database.Receiving
                .Include( static w => w.PendingReceivingTasks )
                .Include( static w => w.ActiveReceivingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            return receiving is not null
                && receiving.StartReceivingTask( employee, taskId )
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
            var receiving = await _database.Receiving
                .Include( static r => r.Pallets )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            return receiving is not null
                && receiving.ReceivePallet( employee, palletId )
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
            var staged = employee
                .GetTask<ReceivingTask>()
                .StagePallet( palletId, areaId );

            return staged
                && await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
    public async Task<bool> CompleteReceivingTask( Employee employee )
    {
        await using var transaction = await GetTransaction();

        try
        {
            var receiving = await _database.Receiving
                .Include( static r => r.ActiveReceivingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            return receiving is not null
                && receiving.CompleteReceivingTask( employee )
                && await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
}