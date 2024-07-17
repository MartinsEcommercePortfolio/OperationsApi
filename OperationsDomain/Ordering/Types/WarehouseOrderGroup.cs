using OperationsDomain.Shipping;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Ordering.Types;

public sealed class WarehouseOrderGroup
{
    public WarehouseOrderGroup() { }
    public WarehouseOrderGroup( Route route, Trailer shippingTrailer, List<WarehouseOrder> orders )
    {
        Route = route;
        ShippingTrailer = shippingTrailer;
        Orders = orders;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Route Route { get; private set; } = default!;
    public Dock Dock { get; private set; } = default!;
    public Area Area { get; private set; } = default!;
    public Trailer ShippingTrailer { get; private set; }
    public List<WarehouseOrder> Orders { get; private set; }
    
    public bool Ship()
    {
        return false;
    }
}