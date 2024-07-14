using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Receiving.Types;

namespace OperationsDomain.Domain.WarehouseSections.Receiving;

internal sealed class ReceivingRepository( WarehouseDbContext dbContext, ILogger<ReceivingRepository> logger ) 
    : DatabaseService<ReceivingRepository>( dbContext, logger ), IReceivingRepository
{
    public async Task<ReceivingSection?> GetReceivingSectionWithTasks()
    {
        try
        {
            return await DbContext.Receiving
                .Include( static w => w.PendingReceivingTasks )
                .Include( static w => w.ActiveReceivingTasks )
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<ReceivingSection?>( e, null );
        }
    }
    public async Task<ReceivingSection?> GetReceivingSectionWithPallets()
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
            return ProcessDbException<ReceivingSection?>( e, null );
        }
    }
}