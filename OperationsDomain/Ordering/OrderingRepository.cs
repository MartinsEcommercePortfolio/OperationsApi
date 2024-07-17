using Microsoft.Extensions.Logging;
using OperationsDomain._Database;
using OperationsDomain.Ordering.Types;

namespace OperationsDomain.Ordering;

internal sealed class OrderingRepository( WarehouseDbContext dbContext, ILogger<OrderingRepository> logger ) 
    : DatabaseService<OrderingRepository>( dbContext, logger ), IOrderingRepository
{
    public Task<OrderingOperations?> GetOrderingOperationsWithOrderGroups() => throw new NotImplementedException();
}