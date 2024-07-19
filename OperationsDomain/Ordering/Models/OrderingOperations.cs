using OperationsDomain.Catalog;

namespace OperationsDomain.Ordering.Models;

public sealed class OrderingOperations
{
    static readonly TimeSpan MaxPendingTime = TimeSpan.FromHours( 8 );

    public Guid Id { get; private set; }
    public List<Product> Products { get; private set; } = [];
    public List<WarehouseOrder> PendingOrders { get; private set; } = [];
    public List<WarehouseOrder> ActiveOrders { get; private set; } = [];
    public List<WarehouseOrder> PickedOrders { get; private set; } = [];
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
    public bool ActivateOrder( WarehouseOrder order )
    {
        var activated = !ActiveOrders.Contains( order )
            && PendingOrders.Remove( order )
            && order.Update( OrderStatus.Fulfilling );

        if (activated)
            ActiveOrders.Add( order );

        return activated;
    }
    public bool CompleteOrder( Guid orderId )
    {
        var order = ActiveOrders.FirstOrDefault( a => a.OrderId == orderId );

        var completed = order is not null
            && ActiveOrders.Remove( order );

        if (completed)
            PickedOrders.Add( order! );
        
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