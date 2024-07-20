using OperationsDomain._Database;
using OperationsDomain.Operations.Putaways.Models;

namespace OperationsDomain.Operations.Putaways;

public interface IPutawayRepository : IEfCoreRepository
{
    public Task<PutawayOperations?> GetPutawaysOperationsWithPalletsAndRackings();
    public Task<PutawayOperations?> GetPutawaysOperationsWithTasks();
}