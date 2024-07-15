using OperationsDomain.Database;
using OperationsDomain.Warehouses.Operations.Receiving.Models;

namespace OperationsDomain.Warehouses.Operations.Receiving;

public interface IReceivingRepository : IEfCoreRepository
{
    public Task<ReceivingOperations?> GetReceivingOperationsWithTasks();
    public Task<ReceivingOperations?> GetReceivingSectionOperationsPallets();
}