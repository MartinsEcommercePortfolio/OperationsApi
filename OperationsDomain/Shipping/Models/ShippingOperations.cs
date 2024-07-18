using OperationsDomain.Ordering.Types;

namespace OperationsDomain.Shipping.Models;

public sealed class ShippingOperations
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Shipment> Shipments { get; private set; } = [];
    public List<ShippingRoute> Routes { get; private set; } = [];

    public Shipment? CreateShipment( ShippingRoute route, List<WarehouseOrder> orders )
    {
        return null;
    }
    public ShippingRoute? GetRouteById( Guid id ) =>
        Routes.FirstOrDefault( r => r.Id == id );
    public ShippingRoute? GetRouteByPos( int posX, int posY ) =>
        Routes.FirstOrDefault( r =>
            r.ContainsAddress( posX, posY ) );
    public bool ShipOrders( Trailer trailer )
    {
        return false;
    }
}