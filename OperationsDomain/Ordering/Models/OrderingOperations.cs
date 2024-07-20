using OperationsDomain.Catalog;

namespace OperationsDomain.Ordering.Models;

public sealed class OrderingOperations
{
    static readonly TimeSpan MaxPendingTime = TimeSpan.FromHours( 8 );

    public Guid Id { get; private set; }
    public List<Product> Products { get; private set; } = [];
    public List<WarehouseOrder> PendingOrders { get; private set; } = [];
    public List<WarehouseOrder> PickingOrders { get; private set; } = [];
    public List<WarehouseOrder> LoadingOrders { get; private set; } = [];
    public List<WarehouseOrder> DispatchedOrders { get; private set; } = [];
    public List<WarehouseOrder> DelayedOrders { get; private set; } = [];
    
    public List<WarehouseOrder> CheckForDelayedOrders()
    {
        List<WarehouseOrder> newlyDelayed = [];
        
        foreach ( var order in PendingOrders )
            if (DateTime.Now - order.DateUpdated > MaxPendingTime && !DelayedOrders.Contains( order ))
                newlyDelayed.Add( order );

        DelayedOrders.AddRange( newlyDelayed );
        return newlyDelayed;
    }
    public bool AddNewOrder( WarehouseOrder warehouseOrder )
    {
        if (!ValidateOrder( warehouseOrder ))
            return false;
        
        PendingOrders.Add( warehouseOrder );
        return true;
    }
    public bool StartPickingOrder( WarehouseOrder order )
    {
        var started = !PickingOrders.Contains( order )
            && !LoadingOrders.Contains( order )
            && !DispatchedOrders.Contains( order )
            && PendingOrders.Remove( order )
            && order.Update( OrderStatus.Fulfilling );

        if (!started)
            return false;

        DelayedOrders.Remove( order );
        PickingOrders.Add( order );

        return true;
    }
    public bool StartLoadingOrder( WarehouseOrder order )
    {
        var started = !PendingOrders.Contains( order )
            && !LoadingOrders.Contains( order )
            && !DispatchedOrders.Contains( order )
            && PickingOrders.Remove( order )
            && order.Update( OrderStatus.Loading );

        if (!started)
            return false;

        DelayedOrders.Remove( order );
        LoadingOrders.Add( order );

        return true;
    }
    public bool DispatchOrder( WarehouseOrder order )
    {
        var dispatched = !PendingOrders.Contains( order )
            && !PickingOrders.Contains( order )
            && !DispatchedOrders.Contains( order )
            && LoadingOrders.Remove( order )
            && order.Update( OrderStatus.Shipped );

        if (!dispatched)
            return false;
        
        DelayedOrders.Remove( order );
        DispatchedOrders.Add( order );
        
        return true;
    }
    bool ValidateOrder( WarehouseOrder order )
    {
        foreach ( var item in order.Items )
        {
            var product = Products
                .FirstOrDefault( p => p.Id == item.ProductId );

            var pickReserved = product is not null
                && product.ReserveQuantity( item.Quantity );

            if (!pickReserved)
                return false;
        }

        return true;
    }
}