using OperationsDomain._Database;
using OperationsDomain.Operations.Ordering.Models;

namespace OperationsDomain.Operations.Ordering;

public interface IOrderingRepository : IEfCoreRepository
{
    public Task<OrderingOperations?> GetOrderingOperationsAll();
}