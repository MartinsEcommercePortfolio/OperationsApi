using OperationsDomain.Database;
using OperationsDomain.Warehouses.Operations.Putaways.Models;

namespace OperationsDomain.Warehouses.Operations.Putaways;

public interface IPutawayRepository : IEfCoreRepository
{
    public Task<PutawayOperations?> GetPutawaysOperationsWithPalletsAndRackings();
    public Task<PutawayOperations?> GetPutawaysOperationsWithTasks();
}