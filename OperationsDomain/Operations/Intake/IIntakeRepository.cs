using OperationsDomain._Database;
using OperationsDomain.Operations.Intake.Models;

namespace OperationsDomain.Operations.Intake;

public interface IIntakeRepository : IEfCoreRepository
{
    public Task<IntakeOperations?> GetReceivingOperationsWithTasks();
    public Task<IntakeOperations?> GetReceivingSectionOperationsPallets();
}