using OperationsDomain.Ordering.Types;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Shipping.Models;

public sealed class Shipment
{
    public Shipment( Trailer trailer, ShippingRoute route, List<Pallet> pallets )
    {
        Trailer = trailer;
        Route = route;
        Pallets = pallets;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Trailer Trailer { get; private set; } 
    public ShippingRoute Route { get; private set; }
    public List<Pallet> Pallets { get; private set; }
    public List<WarehouseOrder> Orders { get; set; }
}