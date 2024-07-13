using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Receiving.Types;

namespace OperationsDomain.Domain.WarehouseSections.Receiving;

public interface IReceivingRepository : IEfCoreRepository
{
    public Task<ReceivingTask?> GetNextReceivingTask();
    public Task<bool> StartReceiving( Employee employee, Guid taskId );
    public Task<bool> ReceivePallet( Employee employee, Guid palletId );
    public Task<bool> StagePallet( Employee employee, Guid palletId, Guid areaId );
}