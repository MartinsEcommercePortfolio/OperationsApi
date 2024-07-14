using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsDomain.Domain.WarehouseSections.Picking;

internal class PickingRepository( WarehouseDbContext dbContext, ILogger<PickingRepository> logger ) 
    : DatabaseService<PickingRepository>( dbContext, logger ), IPickingRepository
{
    readonly WarehouseDbContext _warehouse = dbContext;
    
    public async Task<PickingTask?> GetNextPickingTask()
    {
        try
        {
            var picking = await _warehouse.Picking
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
            return picking?
                .GetNextPickingTask();
        }
        catch ( Exception e )
        {
            return ProcessDbException<PickingTask?>( e, null );
        }
    }
    public async Task<PickingTask?> ResumePickingTask( Employee employee )
    {
        await using var transaction = await GetTransaction();
        
        try
        {
            var task = employee.GetTask<PickingTask>();
            return task;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<PickingTask?>( e, transaction, null );
        }
    }
    public async Task<PickingLine?> StartPickingTask( Employee employee, Guid taskId )
    {
        await using var transaction = await GetTransaction();

        try
        {
            var picking = await _warehouse.Picking
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            var firstPickLocation = picking?
                .StartPickingTask( employee, taskId );

            return firstPickLocation is not null
                && await SaveAsync( transaction )
                    ? firstPickLocation
                    : null;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<PickingLine?>( e, transaction, null );
        }
    }
    public async Task<PickingLine?> GetNextPick( Employee employee )
    {
        await using var transaction = await GetTransaction();

        try
        {
            var nextLocation = employee
                .GetTask<PickingTask>()
                .GetNextPick();

            return nextLocation is not null
                && await SaveAsync( transaction )
                    ? nextLocation
                    : null;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<PickingLine?>( e, transaction, null );
        }
    }
    public async Task<int?> ConfirmPickingLocation( Employee employee, Guid rackingId ) 
    {
        await using var transaction = await GetTransaction();

        try
        {
            var amountToPick = employee
                .GetTask<PickingTask>()
                .ConfirmPickLocation( rackingId );

            return amountToPick is not null
                && await SaveAsync( transaction )
                    ? amountToPick
                    : null;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<int?>( e, transaction, null );
        }
    }
    public async Task<int?> PickItem( Employee employee, Guid itemId )
    {
        await using var transaction = await GetTransaction();

        try
        {
            var itemsLeftInPick = employee
                .GetTask<PickingTask>()
                .PickItem( itemId );

            return itemsLeftInPick is not null
                && await SaveAsync( transaction )
                    ? itemsLeftInPick
                    : null;
        }
        catch ( Exception e )
        {
            return await ProcessDbException<int?>( e, transaction, null );
        }
    }
    public async Task<bool> StagePickingOrder( Employee employee, Guid areaId )
    {
        await using var transaction = await GetTransaction();

        try
        {
            var picking = await _warehouse.Picking
                .Include( static p => p.ActivePickingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );

            return picking is not null
                && picking.StagePickingTask( employee, areaId )
                && await SaveAsync( transaction );
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
}