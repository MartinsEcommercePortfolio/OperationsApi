namespace OperationsApi.Services;

internal sealed class InventoryMonitor : BackgroundService
{
    static readonly TimeSpan ExecutionInterval = TimeSpan.FromMinutes( 20 );
    
    // periodically check inventory and place orders to simulation (through http) for low stock
    protected override Task ExecuteAsync( CancellationToken stoppingToken ) => throw new NotImplementedException();
}