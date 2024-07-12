using OperationsApi.Database;
using OperationsApi.Domain.Warehouses.Loading;

namespace OperationsApi.Features.WarehouseTasks.Loading;

internal sealed class LoadingRepository( WarehouseDbContext dbContext )
{
    readonly WarehouseDbContext _dbContext = dbContext;

    public async Task<LoadingTask?> GetNextTask( Guid employeeId )
    {
        return null;
    }
    public async Task<LoadingTask?> StartTask( Guid employeeId, Guid taskId )
    {
        return null;
    }
}