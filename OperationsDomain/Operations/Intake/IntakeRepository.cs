using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Operations.Intake.Models;

namespace OperationsDomain.Operations.Intake;

internal sealed class IntakeRepository( WarehouseDbContext dbContext, ILogger<IntakeRepository> logger ) 
    : DatabaseService<IntakeRepository>( dbContext, logger ), IIntakeRepository
{
    public async Task<IntakeOperations?> GetReceivingOperationsWithTasks()
    {
        try
        {
            return await DbContext.Receiving
                .Include( static w => w.PendingIntakeTasks )
                .Include( static w => w.ActiveIntakeTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<IntakeOperations?>( e, null );
        }
    }
    public async Task<IntakeOperations?> GetReceivingSectionOperationsPallets()
    {
        try
        {
            return await DbContext.Receiving
                .Include( static r => r.Pallets )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<IntakeOperations?>( e, null );
        }
    }
}