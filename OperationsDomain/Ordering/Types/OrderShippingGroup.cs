using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Ordering.Types;

public sealed class OrderShippingGroup
{
    public OrderShippingGroup() { }
    public OrderShippingGroup( ShippingRoute shippingRoute, Trailer shippingTrailer, List<WarehouseOrder> orders )
    {
        ShippingRoute = shippingRoute;
        ShippingTrailer = shippingTrailer;
        Orders = orders;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public ShippingRoute ShippingRoute { get; private set; } = default!;
    public Dock Dock { get; private set; } = default!;
    public Area Area { get; private set; } = default!;
    public Trailer ShippingTrailer { get; private set; }
    public List<WarehouseOrder> Orders { get; private set; }
    
    public bool Ship()
    {
        return false;
    }
}