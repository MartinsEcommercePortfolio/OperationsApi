using OperationsDomain._Database;
using OperationsDomain.Operations.Shipping.Models;

namespace OperationsDomain.Operations.Shipping;

public interface IShippingRepository : IEfCoreRepository
{
    public Task<ShippingOperations?> GetShippingOperationsWithRoutes();
}