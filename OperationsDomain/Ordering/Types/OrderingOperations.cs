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

    public bool AddOrder( WarehouseOrder warehouseOrder )
    {
        if (!ValidateOrder( warehouseOrder ))
            return false;
        
        PendingOrders.Add( warehouseOrder );
        return true;
    }
    public List<(ShippingRoute, List<WarehouseOrder>)> GetReadyOrdersByRoute()
    {
        Dictionary<ShippingRoute, List<WarehouseOrder>> ordersByRoute = [];

        foreach ( var order in PendingOrders )
        {
            if (!ordersByRoute.TryGetValue( order.ShippingRoute, out List<WarehouseOrder>? orders ))
            {
                orders = [];
                ordersByRoute.Add( order.ShippingRoute, orders );
            }

            orders.Add( order );
        }

        List<(ShippingRoute, List<WarehouseOrder>)> toCreate = [];

        foreach ( var kvp in ordersByRoute )
        {
            if (kvp.Value.Count == OrderGroupMaxSize)
            {
                toCreate.Add( (kvp.Key, kvp.Value) );
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

                toCreate.Add( (kvp.Key, values) );
                break;
            }
        }

        return toCreate;
    }
    public bool ActivateOrder( WarehouseOrder order )
    {
        var activated = !ActiveOrders.Contains( order )
            && PendingOrders.Remove( order );
        
        if (activated)
            ActiveOrders.Add( order );
        
        return false;
    }
    public OrderShippingGroup? CompleteOrder( Guid groupId )
    {
        /*var group = ShippingGroups
            .FirstOrDefault( g => g.Id == groupId );

        var shipped = group is not null
            && group.Ship()
            && ShippingGroups.Remove( group );

        return shipped
            ? group
            : null;*/
        return null;
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