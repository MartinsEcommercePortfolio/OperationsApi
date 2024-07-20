using OperationsDomain._Database;
using OperationsDomain.Operations.Receiving.Models;

namespace OperationsDomain.Operations.Receiving;

public interface IReceivingRepository : IEfCoreRepository
{
    public Task<ReceivingOperations?> GetReceivingOperationsWithTasks();
    public Task<ReceivingOperations?> GetReceivingSectionOperationsPallets();
}