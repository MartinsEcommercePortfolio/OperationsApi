using OperationsDomain._Database;
using OperationsDomain.Ordering.Models;

namespace OperationsDomain.Ordering;

public interface IOrderingRepository : IEfCoreRepository
{
    public Task<OrderingOperations?> GetOrderingOperationsForNewOrder();
}