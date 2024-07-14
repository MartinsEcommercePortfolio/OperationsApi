using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsDomain.Domain.WarehouseSections.Picking;

public interface IPickingRepository : IEfCoreRepository
{
    public Task<PickingTask?> GetNextPickingTask();
    public Task<PickingTask?> ResumePickingTask( Employee employee );
    public Task<PickingLine?> StartPickingTask( Employee employee, Guid taskId );
    public Task<PickingLine?> GetNextPick( Employee employee );
    public Task<int?> ConfirmPickingLocation( Employee employee, Guid rackingId );
    public Task<int?> PickItem( Employee employee, Guid itemId );
    public Task<bool> StagePickingOrder( Employee employee, Guid areaId );
}