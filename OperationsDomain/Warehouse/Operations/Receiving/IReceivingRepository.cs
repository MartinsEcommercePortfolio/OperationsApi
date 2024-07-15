using OperationsDomain._Database;
using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsDomain.Warehouse.Operations.Receiving;

public interface IReceivingRepository : IEfCoreRepository
{
    public Task<ReceivingOperations?> GetReceivingOperationsWithTasks();
    public Task<ReceivingOperations?> GetReceivingSectionOperationsPallets();
}