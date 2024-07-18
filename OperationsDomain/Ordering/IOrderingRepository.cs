using OperationsDomain._Database;
using OperationsDomain.Ordering.Types;

namespace OperationsDomain.Ordering;

public interface IOrderingRepository : IEfCoreRepository
{
    public Task<OrderingOperations?> GetOrderingOperationsForNewOrder();
}