using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Picking.Models;

namespace OperationsDomain.Domain.WarehouseSections.Picking;

public interface IPickingRepository : IEfCoreRepository
{
    public Task<PickingSection?> GetPickingSectionWithTasks();
}