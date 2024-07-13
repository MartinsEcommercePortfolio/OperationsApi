using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;
using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsDomain.Domain.WarehouseSections.Picking;

public interface IPickingRepository : IEfCoreRepository
{
    public Task<PickingTask?> GetNextPickingTask();
    public Task<bool?> BeginPickingTask( Employee employee, Guid taskId );
    public Task<Racking?> GetNextPickLocation( Employee employee );
    public Task<bool> ConfirmPickLocation( Employee employee, Guid rackingId );
    public Task<int?> PickItem( Employee employee, Guid itemId );
}