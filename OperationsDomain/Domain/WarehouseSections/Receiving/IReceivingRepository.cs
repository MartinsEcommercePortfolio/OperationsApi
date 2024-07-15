using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Receiving.Models;

namespace OperationsDomain.Domain.WarehouseSections.Receiving;

public interface IReceivingRepository : IEfCoreRepository
{
    public Task<ReceivingSection?> GetReceivingSectionWithTasks();
    public Task<ReceivingSection?> GetReceivingSectionWithPallets();
}