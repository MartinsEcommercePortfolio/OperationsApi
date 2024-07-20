using OperationsApi.Services.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Operations.Loading;
using OperationsDomain.Operations.Loading.Models;
using OperationsDomain.Operations.Picking;
using OperationsDomain.Operations.Picking.Models;
using OperationsDomain.Operations.Shipping;
using OperationsDomain.Operations.Shipping.Models;
using OperationsDomain.Ordering;
using OperationsDomain.Ordering.Models;

namespace OperationsApi.Services;

internal sealed class OrderingProgresser( 
    IServiceProvider provider,
    ILogger<OrderingProgresser> logger )
    : BackgroundService
{
    readonly IServiceProvider _provider = provider;
    readonly ILogger<OrderingProgresser> _logger = logger;

    static readonly TimeSpan ExecutionInterval = TimeSpan.FromMinutes( 3 );

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        _logger.LogInformation( "OrderingProgresser has started." );
        
        stoppingToken.Register( () =>
            _logger.LogInformation( "OrderingProgresser is stopping." ) );
        
        while ( !stoppingToken.IsCancellationRequested )
        {
            _logger.LogInformation( "OrderingProgresser is executing." );

            try
            {
                await using AsyncServiceScope scope = _provider.CreateAsyncScope();

                var messenger = GetHttpMessenger( scope );
                
                var orderingRepo = GetOrderingRepository( scope );
                var pickingRepo = GetPickingRepository( scope );
                var loadingRepo = GetLoadingRepository( scope );
                var shippingRepo = GetShippingRepository( scope );

                await HandlePendingOrders( orderingRepo, pickingRepo, shippingRepo );
                await HandlePickedOrders( orderingRepo, pickingRepo, loadingRepo );
                await HandleLoadedOrders( messenger, orderingRepo, loadingRepo, shippingRepo );
                await HandleDelayedOrders( messenger, orderingRepo );
            }
            catch ( Exception e )
            {
                _logger.LogError( e, "OrderingProgresser threw an exception during execution." );
            }
            
            await Task.Delay( ExecutionInterval, stoppingToken );
        }

        _logger.LogInformation( "OrderingProgresser has stopped." );
    }
    
    async Task HandlePendingOrders( IOrderingRepository orderingRepo, IPickingRepository pickingRepo, IShippingRepository shippingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();
        var picking = await pickingRepo.GetPickingOperationsWithTasks();
        var shipping = await shippingRepo.GetShippingOperationsWithRoutes();

        if (ordering is null || picking is null || shipping is null)
        {
            _logger.LogError( "OrderingProgresser HandlePendingOrders() failed to generate models from repositories during execution." );
            return;
        }
        
        List<WarehouseOrder> orders = ordering.PendingOrders;

        foreach ( var o in orders )
        {
            await using var transaction = await orderingRepo.Context.Database.BeginTransactionAsync();
            
            // GET TRAILER
            var trailer = shipping.FindAvailableTrailer();
            if (trailer is null)
            {
                _logger.LogWarning( "OrderingProgresser HandlePendingOrders() exited early because no trailers were found during execution." );
                await transaction.RollbackAsync();
                continue;
            }
            o.AssignTrailer( trailer );
            
            // GENERATE TASK
            var task = GeneratePickingTaskForOrder( o, shipping, picking );
            if (task is null)
            {
                _logger.LogWarning( "OrderingProgresser HandlePendingOrders() failed to generate picking task during execution." );
                await transaction.RollbackAsync();
                continue;
            }
            
            // SAVE
            var saved = await orderingRepo.SaveAsync()
                && await pickingRepo.SaveAsync()
                && await shippingRepo.SaveAsync();
    
            if (!saved)
            {
                await transaction.RollbackAsync();
                throw new Exception( "OrderingProgresser HandlePendingOrders() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "OrderingProgresser HandlePendingOrders() successfully handled pending order." );
        }
    }
    async Task HandlePickedOrders( IOrderingRepository orderingRepo, IPickingRepository pickingRepo, ILoadingRepository loadingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();
        var picking = await pickingRepo.GetPickingOperationsWithTasks();
        var loading = await loadingRepo.GetLoadingOperationsWithTasks();

        if (ordering is null || picking is null || loading is null)
        {
            _logger.LogError( "OrderingProgresser HandlePickedOrders() failed to generate models from repositories during execution." );
            return;
        }
        
        var completedPicks = picking.CompletedPickingTasks;
        
        foreach ( PickingTask pick in completedPicks )
        {
            await using var transaction = await orderingRepo.Context.Database.BeginTransactionAsync();
            
            var order = ordering.PickingOrders.FirstOrDefault( o => o.Id == pick.WarehouseOrderId );
            var trailer = loading.Trailers.FirstOrDefault( t => t.Id == order?.TrailerId );
            
            if (order is null || trailer is null)
                continue;
            
            var loadingTask = LoadingTask.New( order.Id, trailer, pick.StagingDock, pick.Pallets );
            var taskGenerated = loading.AddNewTask( loadingTask )
                && picking.RemoveCompletedTask( pick );
            
            if (!taskGenerated)
            {
                _logger.LogWarning( "OrderingProgresser HandlePickedOrders() failed to generate new LoadingTask during execution." );
                await transaction.RollbackAsync();
                continue;
            }

            var saved = await orderingRepo.SaveAsync()
                && await pickingRepo.SaveAsync()
                && await loadingRepo.SaveAsync();

            if (!saved)
            {
                await transaction.RollbackAsync();
                throw new Exception( "OrderingProgresser HandlePickedOrders() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "OrderingProgresser HandlePickedOrders() successfully handled picked order." );
        }
    }
    async Task HandleLoadedOrders( HttpMessenger messenger, IOrderingRepository orderingRepo, ILoadingRepository loadingRepo, IShippingRepository shippingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();
        var loading = await loadingRepo.GetLoadingOperationsWithTasks();
        var shipping = await shippingRepo.GetShippingOperationsWithRoutes();

        if (ordering is null || loading is null || shipping is null)
        {
            _logger.LogError( "OrderingProgresser HandleLoadedOrders() failed to generate models from repositories during execution." );
            return;
        }

        var completedTasks = loading.CompletedLoadingTasks;

        foreach ( var load in completedTasks )
        {
            await using var transaction = await orderingRepo.Context.Database.BeginTransactionAsync();
            
            var order = ordering.PickingOrders.FirstOrDefault( o => o.Id == load.WarehouseOrderId );

            var shipped = order is not null
                && loading.RemoveTask( load )
                && ordering.DispatchOrder( order )
                && await messenger.TryPut<bool>( Consts.ShipToSimulation, null );
            
            if (!shipped)
            {
                _logger.LogError( "OrderingProgresser HandleLoadedOrders() failed to dispatch order during execution." );
                await transaction.RollbackAsync();
                continue;
            }

            var saved = await orderingRepo.SaveAsync()
                && await loadingRepo.SaveAsync()
                && await shippingRepo.SaveAsync();

            if (!saved)
            {
                await transaction.RollbackAsync();
                throw new Exception( "OrderingProgresser HandleLoadedOrders() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "OrderingProgresser HandleLoadedOrders() successfully dispatched order." );
        }
    }
    async Task HandleDelayedOrders( HttpMessenger messenger, IOrderingRepository orderingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();

        if (ordering is null)
        {
            _logger.LogError( "OrderingProgresser failed to get OrderingOperations during execution." );
            return;
        }

        var delayedOrders = ordering.CheckForDelayedOrders();
        var dtos = delayedOrders
            .Select( static o => new OrderDelayedDto {
                OrderId = o.OrderId,
                OrderGroupId = o.OrderGroupId
            } )
            .ToList();

        if (!await messenger.TryPut<bool>( Consts.OrderingDelays, dtos ))
            _logger.LogWarning( "OrderingProgresser delayed order http call failed during execution." );
    }
    
    static PickingTask? GeneratePickingTaskForOrder( WarehouseOrder order, ShippingOperations shipping, PickingOperations picking )
    {
        var dock = shipping.FindAvailableDock();

        if (dock is null || dock.IsOwned())
            return null;

        var productIds = order.Items
            .Select( static i => i.ProductId )
            .ToList();
        
        return picking.GenerateNewPickingTask( order.Id, dock, productIds );
    }
    static HttpMessenger GetHttpMessenger( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<HttpMessenger>() ?? throw new Exception( $"Failed to get {nameof( HttpMessenger )} from provider." );
    static IOrderingRepository GetOrderingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IOrderingRepository>() ?? throw new Exception( $"Failed to get {nameof( IOrderingRepository )} from provider." );
    static IPickingRepository GetPickingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IPickingRepository>() ?? throw new Exception( $"Failed to get {nameof( IPickingRepository )} from provider." );
    static ILoadingRepository GetLoadingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<ILoadingRepository>() ?? throw new Exception( $"Failed to get {nameof( ILoadingRepository )} from provider." );
    static IShippingRepository GetShippingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IShippingRepository>() ?? throw new Exception( $"Failed to get {nameof( IShippingRepository )} from provider." );
}