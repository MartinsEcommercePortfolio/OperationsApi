using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Shipping.Models;

public sealed class ShippingLoad
{
    public ShippingLoad( Guid orderId, Pallet pallet )
    {
        OrderId = orderId;
        Pallet = pallet;
    }
    
    public Guid OrderId { get; private set; }
    public Pallet Pallet { get; private set; }
}