using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Putaways;

public interface IPutawayRepository : IEfCoreRepository
{
    public Task<Racking?> StartPutawayTask( Employee employee, Guid palletId );
    public Task<bool> CompletePutawayTask( Employee employee, Guid palletId, Guid rackingId );
}