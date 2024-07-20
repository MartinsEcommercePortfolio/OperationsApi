using OperationsDomain._Database;
using OperationsDomain.Outbound.Shipping.Models;

namespace OperationsDomain.Outbound.Shipping;

public interface IShippingRepository : IEfCoreRepository
{
    public Task<ShippingOperations?> GetShippingOperationsWithRoutes();
}