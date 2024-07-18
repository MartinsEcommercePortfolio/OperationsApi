using OperationsDomain.Catalog;
using OperationsDomain.Shipping.Models;

namespace OperationsDomain.Ordering.Types;

public sealed class OrderingOperations
{
    const int OrderGroupMaxSize = 12;
    static readonly TimeSpan MaxPendingTime = TimeSpan.FromHours( 8 );

    public Guid Id { get; private set; } = Guid.NewGuid();
    public List<Product> Products { get; private set; } = [];
    public List<WarehouseOrder> PendingOrders { get; private set; } = [];
    public List<WarehouseOrder> ActiveOrders { get; private set; } = [];
    public List<WarehouseOrder> PickedOrders { get; private set; } = [];

    public bool AddOrder( WarehouseOrder warehouseOrder )
    {
        if (!ValidateOrder( warehouseOrder ))
            return false;
        
        PendingOrders.Add( warehouseOrder );
        return true;
    }
    public Dictionary<Guid, List<WarehouseOrder>> GetReadyOrdersByRoute()
    {
        Dictionary<Guid, List<WarehouseOrder>> ordersByRouteId = [];

        foreach ( var order in PendingOrders )
        {
            if (!ordersByRouteId.TryGetValue( order.ShippingRouteId, out List<WarehouseOrder>? orders ))
            {
                orders = [];
                ordersByRouteId.Add( order.ShippingRouteId, orders );
            }

            orders.Add( order );
        }

        Dictionary<Guid, List<WarehouseOrder>> toReturn = [];

        foreach ( var kvp in ordersByRouteId )
        {
            if (kvp.Value.Count == OrderGroupMaxSize)
            {
                toReturn.Add( kvp.Key, kvp.Value );
                continue;
            }

            foreach ( var order in kvp.Value )
            {
                if (DateTime.Now - order.DateReceived < MaxPendingTime)
                    continue;

                var values = kvp.Value
                    .OrderBy( static o => o.DateCreated )
                    .Take( OrderGroupMaxSize )
                    .ToList();

                toReturn.Add( kvp.Key, values );
                break;
            }
        }

        return toReturn;
    }
    public bool ActivateOrder( WarehouseOrder order )
    {
        var activated = !ActiveOrders.Contains( order )
            && PendingOrders.Remove( order );
        
        if (activated)
            ActiveOrders.Add( order );
        
        return false;
    }
    public bool CompleteOrder( Guid orderId )
    {
        var order = ActiveOrders.FirstOrDefault( a => a.OrderId == orderId );

        var completed = order is not null
            && ActiveOrders.Remove( order );

        if (completed)
            PickedOrders.Add( order! );

        return completed;
    }

    bool ValidateOrder( WarehouseOrder order )
    {
        foreach ( var item in order.Items )
        {
            var product = Products
                .FirstOrDefault( p => p.Id == item.ProductId );

            var pickReserved = product is not null
                && product.ReservePickAmount( item.Quantity );

            if (!pickReserved)
                return false;
        }

        return true;
    }
    Trailer? FindTrailerForRoute( ShippingRoute shippingRoute, List<Trailer> trailers )
    {
        return null;
    }
}