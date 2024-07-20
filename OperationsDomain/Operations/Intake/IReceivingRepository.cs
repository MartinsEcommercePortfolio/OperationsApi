using OperationsDomain._Database;
using OperationsDomain.Operations.Intake.Models;

namespace OperationsDomain.Operations.Intake;

public interface IReceivingRepository : IEfCoreRepository
{
    public Task<ReceivingOperations?> GetReceivingOperationsWithTasks();
    public Task<ReceivingOperations?> GetReceivingSectionOperationsPallets();
}