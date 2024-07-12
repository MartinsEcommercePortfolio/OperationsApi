using Microsoft.EntityFrameworkCore.Storage;

namespace OperationsApi.Database;

internal abstract class DatabaseService<TService>( WarehouseDbContext dbContext, ILogger<TService> logger )
{
    protected readonly ILogger<TService> Logger = logger;
    protected readonly WarehouseDbContext DbContext = dbContext;
    
    public async Task<bool> SaveAsync()
    {
        try
        {
            return await DbContext.SaveChangesAsync() > 0;
        }
        catch ( Exception e )
        {
            return ProcessDbException( e, false );
        }
    }
    public async Task<bool> SaveAsync( IDbContextTransaction transaction )
    {
        try
        {
            bool changesSaved = await SaveAsync();
            if (changesSaved)
                await transaction.CommitAsync().ConfigureAwait( false );
            return changesSaved;
        }
        catch ( Exception e )
        {
            return await ProcessDbException( e, transaction, false );
        }
    }
    protected async Task<IDbContextTransaction> GetTransaction() =>
        await DbContext.Database
            .BeginTransactionAsync()
            .ConfigureAwait( false );
    protected T ProcessDbException<T>( Exception e, T returnValue )
    {
        Logger.LogError( e, e.Message );
        return returnValue;
    }
    protected async Task<T> ProcessDbException<T>( Exception e, IDbContextTransaction transaction, T returnValue )
    {
        Logger.LogError( e, e.Message );
        await transaction
            .RollbackAsync()
            .ConfigureAwait( false );
        return returnValue;
    }
}