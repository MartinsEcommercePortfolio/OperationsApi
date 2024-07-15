using OperationsDomain.Database;
using OperationsDomain.Domain.WarehouseSections.Putaways.Models;

namespace OperationsDomain.Domain.WarehouseSections.Putaways;

public interface IPutawayRepository : IEfCoreRepository
{
    public Task<PutawaySection?> GetPutawaysSectionWithPalletsAndRackings();
    public Task<PutawaySection?> GetPutawaysSectionWithTasks();
}