using OperationsDomain._Database;
using OperationsDomain.Warehouse.Operations.Putaways.Models;

namespace OperationsDomain.Warehouse.Operations.Putaways;

public interface IPutawayRepository : IEfCoreRepository
{
    public Task<PutawayOperations?> GetPutawaysOperationsWithPalletsAndRackings();
    public Task<PutawayOperations?> GetPutawaysOperationsWithTasks();
}