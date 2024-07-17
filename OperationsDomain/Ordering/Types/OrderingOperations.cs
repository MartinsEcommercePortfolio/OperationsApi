using OperationsDomain.Catalog;
using OperationsDomain.Shipping;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Ordering.Types;

public sealed class OrderingOperations
{
    const int OrderGroupMaxSize = 12;
    static readonly TimeSpan MaxPendingTime = TimeSpan.FromHours( 8 );

    public Guid Id { get; private set; } = Guid.NewGuid();
    public List<Route> Routes { get; private set; } = [];
    public List<Product> Products { get; private set; } = [];
    public List<WarehouseOrder> PendingOrders { get; private set; } = [];
    public List<WarehouseOrderGroup> OrderGroups { get; private set; } = [];

    public bool AddOrder( WarehouseOrder warehouseOrder )
    {
        if (!ValidateOrder( warehouseOrder ))
            return false;
        
        PendingOrders.Add( warehouseOrder );
        return true;
        /*

        if (route is null)
            return false;

        var orderGroup = GetOrCreateOrderGroup( warehouseOrder, route );
        return orderGroup is not null;*/
    }
    public List<WarehouseOrderGroup> GenerateOrderGroups()
    {
        Dictionary<Route, HashSet<WarehouseOrder>> ordersByRoute = [];

        foreach ( var order in PendingOrders )
        {
            if (order.Route is null)
                continue;

            if (!ordersByRoute.TryGetValue( order.Route, out HashSet<WarehouseOrder>? orders ))
            {
                orders = [];
                ordersByRoute.Add( order.Route, orders );
            }

            orders.Add( order );
        }

        List<KeyValuePair<Route, HashSet<WarehouseOrder>>> toCreate = [];

        foreach ( KeyValuePair<Route, HashSet<WarehouseOrder>> kvp in ordersByRoute )
        {
            if (kvp.Value.Count == OrderGroupMaxSize)
            {
                toCreate.Add( kvp );
                continue;
            }
            
            foreach ( var order in kvp.Value )
            {
                if (DateTime.Now - order.DateReceived <= MaxPendingTime) 
                    continue;
                
                toCreate.Add( kvp );
                break;
            }
        }

        List<WarehouseOrderGroup> newGroups = [];
        
        foreach ( var kvp in toCreate )
        {
            var group = CreateOrderGroup( kvp.Key, kvp.Value );
            
            if (group is null) 
                continue;
            
            OrderGroups.Add( group );
            newGroups.Add( group );
        }

        return newGroups;
    }
    public WarehouseOrderGroup? ShipOrderGroup( Guid groupId )
    {
        var group = OrderGroups
            .FirstOrDefault( g => g.Id == groupId );

        var shipped = group is not null
            && group.Ship()
            && OrderGroups.Remove( group );

        return shipped
            ? group
            : null;
    }

    bool ValidateOrder( WarehouseOrder order )
    {
        foreach ( var item in order.Items )
        {
            var route = FindRouteForOrder( order );
            var product = Products.FirstOrDefault( p => p.Id == item.ProductId );
            if (product is null || product.Stock < item.Quantity || route is null)
                return false;
            product.Stock -= item.Quantity;
            order.SetRoute( route );
        }

        return true;
    }
    Route? FindRouteForOrder( WarehouseOrder order )
    {
        return Routes.FirstOrDefault( r =>
            r.ContainsAddress( order.PosX, order.PosX ) );
    }
    Trailer? FindTrailerForRoute( Route route )
    {
        return null;
    }
    WarehouseOrderGroup? CreateOrderGroup( Route route, HashSet<WarehouseOrder> orders )
    {
        var trailer = FindTrailerForRoute( route );

        if (trailer is null)
            return null;

        var orderGroup = OrderGroups.FirstOrDefault( o =>
            o.Route == route ) ?? new WarehouseOrderGroup( route, trailer, orders.ToList() );
        
        return orderGroup;
    }
}