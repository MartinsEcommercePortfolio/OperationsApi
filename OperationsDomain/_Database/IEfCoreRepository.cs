namespace OperationsDomain._Database;

public interface IEfCoreRepository
{
    public Task<bool> SaveAsync();
}