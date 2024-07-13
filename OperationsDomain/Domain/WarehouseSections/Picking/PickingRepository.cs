using Microsoft.Extensions.Logging;
using OperationsDomain.Database;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;
using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsDomain.Domain.WarehouseSections.Picking;

internal class PickingRepository( WarehouseDbContext dbContext, ILogger<PickingRepository> logger ) 
    : DatabaseService<PickingRepository>( dbContext, logger ), IPickingRepository
{
    public async Task<PickingTask?> GetNextPickingTask() => throw new NotImplementedException();
    public async Task<bool?> BeginPickingTask( Employee employee, Guid taskId ) => throw new NotImplementedException();
    public async Task<Racking?> GetNextPickLocation( Employee employee ) => throw new NotImplementedException();
    public async Task<bool> ConfirmPickLocation( Employee employee, Guid rackingId ) => throw new NotImplementedException();
    public async Task<int?> PickItem( Employee employee, Guid itemId ) => throw new NotImplementedException();
}