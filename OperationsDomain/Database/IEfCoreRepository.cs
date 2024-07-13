namespace OperationsDomain.Database;

public interface IEfCoreRepository
{
    public Task<bool> SaveAsync();
}