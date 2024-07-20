using OperationsDomain._Database;
using OperationsDomain.Inbound.Putaways.Models;

namespace OperationsDomain.Inbound.Putaways;

public interface IPutawayRepository : IEfCoreRepository
{
    public Task<PutawayOperations?> GetPutawaysOperationsWithPalletsAndRackings();
    public Task<PutawayOperations?> GetPutawaysOperationsWithTasks();
}