using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Shipping.Models;

public sealed class Shipment
{
    public Shipment( Trailer trailer, ShippingRoute route, List<Guid> orderIds, List<Pallet> pallets )
    {
        Trailer = trailer;
        Route = route;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Trailer Trailer { get; private set; } 
    public ShippingRoute Route { get; private set; }
    public List<ShippingLoad> Loads { get; private set; } = [];
}