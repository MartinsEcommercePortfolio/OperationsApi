using Microsoft.EntityFrameworkCore;

namespace OperationsDomain._Database;

public interface IEfCoreRepository
{
    public DbContext Context { get; }
    public bool ClearChanges();
    public Task<bool> SaveAsync();
}