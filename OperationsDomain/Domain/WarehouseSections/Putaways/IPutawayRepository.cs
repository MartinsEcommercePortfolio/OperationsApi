using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Putaways;

public interface IPutawayRepository : IEfCoreRepository
{
    public Task<Racking?> BeginPutaway( Employee employee, Guid palletId );
    public Task<bool> CompletePutaway( Employee employee, Guid palletId, Guid rackingId );
}