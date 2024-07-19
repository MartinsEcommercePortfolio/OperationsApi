using OperationsApi.Services.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Ordering;
using OperationsDomain.Ordering.Models;
using OperationsDomain.Shipping;
using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Operations.Picking;
using OperationsDomain.Warehouse.Operations.Picking.Models;

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
                var shippingRepo = GetShippingRepository( scope );

                await HandlePendingOrders( http, orderingRepo, pickingRepo, shippingRepo );
                await HandlePickedOrders( http, orderingRepo, pickingRepo, shippingRepo );
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

        if (ordering is null || shipping is null || picking is null)
        {
            _logger.LogError( "OrderingDispatcher failed to generate models from repositories during execution." );
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

            if (!await orderingRepo.SaveAsync() || !await pickingRepo.SaveAsync() || !await shippingRepo.SaveAsync())
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
    async Task HandlePickedOrders( HttpHandler http, IOrderingRepository orderingRepo, IPickingRepository pickingRepo, IShippingRepository shippingRepo )
    {
        // call to http (simulation) to take control of trailer (with its load)
        // if (respond success) remove the order and notify ordering api orders have been shipped
        // subtract item quantities from real product counts respectively (update inventory)
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
    static IShippingRepository GetShippingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IShippingRepository>() ?? throw new Exception( $"Failed to get {nameof( IShippingRepository )} from provider." );
    static IPickingRepository GetPickingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IPickingRepository>() ?? throw new Exception( $"Failed to get {nameof( IPickingRepository )} from provider." );
}