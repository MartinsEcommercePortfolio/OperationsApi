using OperationsDomain.Ordering;
using OperationsDomain.Ordering.Types;
using OperationsDomain.Shipping;
using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Operations.Picking;
using OperationsDomain.Warehouse.Operations.Picking.Models;

namespace OperationsApi.Services;

internal sealed class PickingTaskGenerator( IOrderingRepository orderingRepository, IShippingRepository shippingRepository, IPickingRepository pickingRepository )
{
    readonly IOrderingRepository _orderingRepository = orderingRepository;
    readonly IShippingRepository _shippingRepository = shippingRepository;
    readonly IPickingRepository _pickingRepository = pickingRepository;

    async Task GeneratePickTasks()
    {
        var ordering = await _orderingRepository.GetOrderingOperationsForNewOrder();
        var shipping = await _shippingRepository.GetShippingOperationsWithRoutes();
        var picking = await _pickingRepository.GetPickingOperationsWithTasks();

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
            return;

        await _orderingRepository.SaveAsync();
        await _shippingRepository.SaveAsync();
        await _pickingRepository.SaveAsync();
    }

    static List<Shipment>? CreateShipments( List<(ShippingRoute, List<WarehouseOrder>)> orders, ShippingOperations shipping )
    {
        List<Shipment> shipments = [];
        
        foreach ( var o in orders )
        {
            var shipment = shipping.CreateShipment( o );
            
            if (shipment is null)
                return null;
            
            shipments.Add( shipment );
        }
        
        return shipments;
    }
    static List<PickingTask>? CreatePickingTasks( List<(ShippingRoute, List<WarehouseOrder>)> orders, PickingOperations picking )
    {
        List<PickingTask> tasks = [];

        foreach ( (ShippingRoute, List<WarehouseOrder>) o in orders )
        {
            foreach ( WarehouseOrder wo in o.Item2 )
            {
                var task = picking.GeneratePickingTask( wo.OrderId, wo.Items
                    .Select( static w => (w.ProductId, w.Quantity) )
                    .ToList() );

                if (task is null)
                    return null;
                
                tasks.Add( task );
            }
        }
        
        return tasks;
    }
    static bool ActivateOrders( List<(ShippingRoute, List<WarehouseOrder>)> orders, OrderingOperations ordering )
    {
        foreach ( var o in orders )
            foreach ( var wo in o.Item2 )
                if (!ordering.ActivateOrder( wo ))
                    return false;
        return true;
    }
    static bool AddPendingTasks( List<PickingTask> tasks, PickingOperations picking )
    {
        foreach ( var t in tasks )
            if (!picking.AddPendingTask( t ))
                return false;
        return true;
    }
}