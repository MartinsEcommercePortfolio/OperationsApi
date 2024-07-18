using OperationsDomain._Database;
using OperationsDomain.Shipping.Models;

namespace OperationsDomain.Shipping;

public interface IShippingRepository : IEfCoreRepository
{
    public Task<ShippingOperations?> GetShippingOperationsWithRoutes();
}