using OperationsApi.Utilities;
using OperationsDomain.Ordering;
using OperationsDomain.Ordering.Types;
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

    static readonly TimeSpan ExecutionInterval = TimeSpan.FromMinutes( 10 );

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        _logger.LogInformation( "OrderingDispatcher has started." );
        
        stoppingToken.Register( () =>
            _logger.LogInformation( "OrderingDispatcher is stopping." ) );
        
        while ( !stoppingToken.IsCancellationRequested )
        {
            _logger.LogInformation( "OrderingDispatcher is executing." );

            //await HandlePendingOrders();
            //await HandleCompletedOrders();
            await Task.Delay( ExecutionInterval, stoppingToken );
        }

        _logger.LogInformation( "OrderingDispatcher has stopped." );
    }
    
    /*async Task HandlePendingOrders()
    {
        await using AsyncServiceScope scope = _provider.CreateAsyncScope();

        var http = GetHttpHandler( scope );
        var orderingRepo = GetOrderingRepository( scope );
        var shippingRepo = GetShippingRepository( scope );
        var pickingRepo = GetPickingRepository( scope );
        
        var ordering = await orderingRepo.GetOrderingOperationsForNewOrder();
        var shipping = await shippingRepo.GetShippingOperationsWithRoutes();
        var picking = await pickingRepo.GetPickingOperationsWithTasks();

        if (ordering is null || shipping is null || picking is null)
            return;

        var orders = ordering.GetReadyOrdersByRoute();
        var shipments = CreateShipments( orders, shipping );
        var tasks = CreatePickingTasks( orders, picking );

        var success = shipments is not null
            && tasks is not null
            && ActivateOrders( orders, ordering )
            && AddPendingTasks( tasks, picking );

        if (!success)
            throw new Exception( "OrderingDispatcher failed to handle pending orders." );

        await orderingRepo.SaveAsync();
        await shippingRepo.SaveAsync();
        await pickingRepo.SaveAsync();
    }
    async Task HandleCompletedOrders()
    {
        // call to http (simulation) to take control of trailer (with its load)
        // if (respond success) remove the order and notify ordering api orders have been shipped
        // subtract item quantities from real product counts respectively (update inventory)
    }

    static List<Shipment> CreateShipments( Dictionary<Guid, List<WarehouseOrder>> ordersByRouteId, ShippingOperations shipping )
    {
        List<Shipment> shipments = [];
        
        foreach ( var kvp in ordersByRouteId )
        {
            var shippingRoute = shipping.GetRouteById( kvp.Key );
            ArgumentNullException.ThrowIfNull( shippingRoute );
            
            var shipment = shipping.CreateShipment( shippingRoute, kvp.Value );
            ArgumentNullException.ThrowIfNull( shipment );
            
            shipments.Add( shipment );
        }
        
        return shipments;
    }
    static List<PickingTask>? CreatePickingTasks( Dictionary<Guid, List<WarehouseOrder>> ordersByRouteId, PickingOperations picking )
    {
        List<PickingTask> tasks = [];

        foreach ( var kvp in ordersByRouteId )
            foreach ( var order in kvp.Value )
            {
                var task = picking.GeneratePickingTask( order.OrderId, order.Items
                    .Select( static w => (w.ProductId, w.Quantity) )
                    .ToList() );
                ArgumentNullException.ThrowIfNull( task );
                
                tasks.Add( task );
            }
        
        return tasks;
    }
    static bool ActivateOrders( Dictionary<Guid, List<WarehouseOrder>> ordersByRouteId, OrderingOperations ordering )
    {
        foreach ( var kvp in ordersByRouteId )
            foreach ( var order in kvp.Value )
                if (!ordering.ActivateOrder( order ))
                    throw new Exception( $"Failed to activate order: {order} {order.OrderId}" );
        
        return true;
    }
    static bool AddPendingTasks( List<PickingTask> tasks, PickingOperations picking )
    {
        foreach ( var t in tasks )
            if (!picking.AddPendingTask( t ))
                throw new Exception( $"Failed to add pending picking task: {t} {t.Employee}" );
        
        return true;
    }

    HttpHandler GetHttpHandler( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<HttpHandler>() ?? throw new Exception( $"Failed to get {nameof( HttpHandler )} from provider." );
    IOrderingRepository GetOrderingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IOrderingRepository>() ?? throw new Exception( $"Failed to get {nameof( IOrderingRepository )} from provider." );
    IShippingRepository GetShippingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IShippingRepository>() ?? throw new Exception( $"Failed to get {nameof( IShippingRepository )} from provider." );
    IPickingRepository GetPickingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IPickingRepository>() ?? throw new Exception( $"Failed to get {nameof( IPickingRepository )} from provider." );*/
}