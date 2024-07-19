using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace OperationsDomain._Database;

public abstract class DatabaseService<TService>( WarehouseDbContext dbContext, ILogger<TService> logger )
{
    protected readonly ILogger<TService> Logger = logger;
    protected readonly WarehouseDbContext DbContext = dbContext;

    public DbContext Context => DbContext;

    public bool ClearChanges()
    {
        var entries = DbContext.ChangeTracker.Entries()
            .Where( static e => e.State != EntityState.Unchanged )
            .ToList();
        
        foreach ( var entry in entries )
        {
            switch ( entry.State )
            {
                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.Reload();
                    break;
                case EntityState.Detached:
                    throw new ArgumentOutOfRangeException();
                case EntityState.Unchanged:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return true;
    }
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