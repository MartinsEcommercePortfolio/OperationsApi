namespace OperationsApi.Database;

public interface IEfCoreRepository
{
    public Task<bool> SaveAsync();
}