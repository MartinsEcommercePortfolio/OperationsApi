using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Operations.Inbound.Models;

namespace OperationsDomain.Operations.Inbound;

internal sealed class InboundRepository( WarehouseDbContext dbContext, ILogger<InboundRepository> logger ) 
    : DatabaseService<InboundRepository>( dbContext, logger ), IInboundRepository
{
    public async Task<InboundOperations?> GetReceivingOperations()
    {
        try
        {
            return await DbContext.Receiving
                .FirstOrDefaultAsync()
                .ConfigureAwait( false );
        }
        catch ( Exception e )
        {
            return ProcessDbException<InboundOperations?>( e, null );
        }
    }
}