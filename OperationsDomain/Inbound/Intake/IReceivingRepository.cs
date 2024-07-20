using OperationsDomain._Database;
using OperationsDomain.Inbound.Intake.Models;

namespace OperationsDomain.Inbound.Intake;

public interface IReceivingRepository : IEfCoreRepository
{
    public Task<ReceivingOperations?> GetReceivingOperationsWithTasks();
    public Task<ReceivingOperations?> GetReceivingSectionOperationsPallets();
}