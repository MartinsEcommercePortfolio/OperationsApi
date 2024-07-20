using OperationsApi.Services.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Operations.Loading;
using OperationsDomain.Operations.Loading.Models;
using OperationsDomain.Operations.Ordering;
using OperationsDomain.Operations.Ordering.Models;
using OperationsDomain.Operations.Picking;
using OperationsDomain.Operations.Picking.Models;
using OperationsDomain.Operations.Shipping;
using OperationsDomain.Operations.Shipping.Models;

namespace OperationsApi.Services;

internal sealed class OrderingDispatcher( 
    IServiceProvider provider,
    ILogger<OrderingDispatcher> logger )
    : BackgroundService
{
    readonly IServiceProvider _provider = provider;
    readonly ILogger<OrderingDispatcher> _logger = logger;

    static readonly TimeSpan ExecutionInterval = TimeSpan.FromMinutes( 3 );

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        _logger.LogInformation( "OrderingDispatcher has started." );
        
        stoppingToken.Register( () =>
            _logger.LogInformation( "OrderingDispatcher is stopping." ) );
        
        while ( !stoppingToken.IsCancellationRequested )
        {
            _logger.LogInformation( "OrderingDispatcher is executing." );

            try
            {
                await using AsyncServiceScope scope = _provider.CreateAsyncScope();

                var http = GetHttpHandler( scope );
                
                var orderingRepo = GetOrderingRepository( scope );
                var pickingRepo = GetPickingRepository( scope );
                var loadingRepo = GetLoadingRepository( scope );
                var shippingRepo = GetShippingRepository( scope );

                await HandlePendingOrders( http, orderingRepo, pickingRepo, shippingRepo );
                await HandlePickedOrders( http, orderingRepo, pickingRepo, loadingRepo );
                await HandleLoadedOrders( http, orderingRepo, loadingRepo, shippingRepo );
                await HandleDelayedOrders( http, orderingRepo );
            }
            catch ( Exception e )
            {
                _logger.LogError( e, "OrderingDispatcher threw an exception during execution." );
            }
            
            await Task.Delay( ExecutionInterval, stoppingToken );
        }

        _logger.LogInformation( "OrderingDispatcher has stopped." );
    }
    
    async Task HandlePendingOrders( HttpHandler http, IOrderingRepository orderingRepo, IPickingRepository pickingRepo, IShippingRepository shippingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();
        var picking = await pickingRepo.GetPickingOperationsWithTasks();
        var shipping = await shippingRepo.GetShippingOperationsWithRoutes();

        if (ordering is null || picking is null || shipping is null)
        {
            _logger.LogError( "OrderingDispatcher HandlePendingOrders() failed to generate models from repositories during execution." );
            return;
        }
        
        List<WarehouseOrder> orders = ordering.PendingOrders;

        foreach ( var o in orders )
        {
            await using var transaction = await orderingRepo.Context.Database.BeginTransactionAsync();
            
            var trailer = shipping.FindAvailableTrailer();
            if (trailer is null)
            {
                _logger.LogWarning( "OrderingDispatcher HandlePendingOrders() exited early because no trailers were found during execution." );
                await transaction.RollbackAsync();
                return;
            }
            
            o.AssignTrailer( trailer );
            var task = GeneratePickingTaskForOrder( o, shipping, picking );
            if (task is null)
            {
                _logger.LogWarning( "OrderingDispatcher HandlePendingOrders() failed to generate picking task during execution." );
                await transaction.RollbackAsync();
                continue;
            }
            
            if (!ordering.ActivateOrder( o ))
            {
                _logger.LogWarning( "OrderingDispatcher HandlePendingOrders() failed to activate order during execution." );
                await transaction.RollbackAsync();
                continue;
            }

            var saved = await orderingRepo.SaveAsync()
                && await pickingRepo.SaveAsync()
                && await shippingRepo.SaveAsync();

            if (!saved)
            {
                await transaction.RollbackAsync();
                throw new Exception( "OrderingDispatcher HandlePendingOrders() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "OrderingDispatcher HandlePendingOrders() successfully handled pending order." );

            if (!await http.TryPut<bool>( Consts.OrderingUpdate, new OrderUpdateDto( o.OrderId, o.OrderGroupId, 0 ) ))
                _logger.LogWarning( "OrderingDispatcher HandlePendingOrders() order update http call failed during execution." );
        }
    }
    async Task HandlePickedOrders( HttpHandler http, IOrderingRepository orderingRepo, IPickingRepository pickingRepo, ILoadingRepository loadingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();
        var picking = await pickingRepo.GetPickingOperationsWithTasks();
        var loading = await loadingRepo.GetLoadingOperationsWithTasks();

        if (ordering is null || picking is null || loading is null)
        {
            _logger.LogError( "OrderingDispatcher HandlePickedOrders() failed to generate models from repositories during execution." );
            return;
        }
        
        var completedPicks = picking.CompletedPickingTasks;
        
        foreach ( PickingTask pick in completedPicks )
        {
            await using var transaction = await orderingRepo.Context.Database.BeginTransactionAsync();
            
            var order = ordering.FulfillingOrders.FirstOrDefault( o => o.Id == pick.WarehouseOrderId );
            var trailer = loading.Trailers.FirstOrDefault( t => t.Id == order?.TrailerId );
            
            if (order is null || trailer is null)
                continue;
            
            var loadingTask = LoadingTask.New( order.Id, trailer, pick.StagingDock, pick.Pallets );
            var taskGenerated = loading.AddNewTask( loadingTask )
                && picking.RemoveCompletedTask( pick );
            
            if (!taskGenerated)
            {
                _logger.LogWarning( "OrderingDispatcher HandlePickedOrders() failed to generate new LoadingTask during execution." );
                await transaction.RollbackAsync();
                continue;
            }

            var saved = await orderingRepo.SaveAsync()
                && await pickingRepo.SaveAsync()
                && await loadingRepo.SaveAsync();

            if (!saved)
            {
                await transaction.RollbackAsync();
                throw new Exception( "OrderingDispatcher HandlePickedOrders() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "OrderingDispatcher HandlePickedOrders() successfully handled picked order." );
        }
    }
    async Task HandleLoadedOrders( HttpHandler http, IOrderingRepository orderingRepo, ILoadingRepository loadingRepo, IShippingRepository shippingRepo )
    {
        // call to http (simulation) to take control of trailer (with its load)
        // if (respond success) remove the order and notify ordering api orders have been shipped
        // subtract item quantities from real product counts respectively (update inventory)
        
        OrderingOperations? ordering = await orderingRepo.GetOrderingOperationsAll();
        var loading = await loadingRepo.GetLoadingOperationsWithTasks();
        var shipping = await shippingRepo.GetShippingOperationsWithRoutes();

        if (ordering is null || loading is null || shipping is null)
        {
            _logger.LogError( "OrderingDispatcher HandleLoadedOrders() failed to generate models from repositories during execution." );
            return;
        }

        var completedLoads = loading.CompletedLoadingTasks;

        foreach ( var load in completedLoads )
        {
            var order = ordering.FulfillingOrders.FirstOrDefault( o => o.Id == load.WarehouseOrderId );

            if (order is null)
            {
                _logger.LogError( "OrderingDispatcher HandleLoadedOrders() failed to find order for load during execution." );
                continue;
            }
            
            var simulationAcceptedShipment = await http.TryPut<bool>( Consts.ShipToSimulation, null );

            if (!simulationAcceptedShipment)
            {
                _logger.LogError( "OrderingDispatcher HandleLoadedOrders() http simulationAcceptedShipment failed load during execution." );
                continue;
            }
            
            
        }
    }
    async Task HandleDelayedOrders( HttpHandler http, IOrderingRepository orderingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();

        if (ordering is null)
        {
            _logger.LogError( "OrderingDispatcher failed to get OrderingOperations during execution." );
            return;
        }

        var delayedOrders = ordering.CheckForDelayedOrders();
        var dtos = delayedOrders
            .Select( static o => new OrderDelayedDto {
                OrderId = o.OrderId,
                OrderGroupId = o.OrderGroupId
            } )
            .ToList();

        if (!await http.TryPut<bool>( Consts.OrderingDelays, dtos ))
            _logger.LogWarning( "OrderingDispatcher delayed order http call failed during execution." );
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
    static HttpHandler GetHttpHandler( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<HttpHandler>() ?? throw new Exception( $"Failed to get {nameof( HttpHandler )} from provider." );
    static IOrderingRepository GetOrderingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IOrderingRepository>() ?? throw new Exception( $"Failed to get {nameof( IOrderingRepository )} from provider." );
    static IPickingRepository GetPickingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IPickingRepository>() ?? throw new Exception( $"Failed to get {nameof( IPickingRepository )} from provider." );
    static ILoadingRepository GetLoadingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<ILoadingRepository>() ?? throw new Exception( $"Failed to get {nameof( ILoadingRepository )} from provider." );
    static IShippingRepository GetShippingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IShippingRepository>() ?? throw new Exception( $"Failed to get {nameof( IShippingRepository )} from provider." );
}