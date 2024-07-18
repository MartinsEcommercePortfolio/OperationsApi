using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Ordering.Types;

public sealed class WarehouseOrder
{
    public WarehouseOrder( Guid orderId, Guid orderGroupId, ShippingRoute shippingRoute, string? customerId, DateTime dateCreated, int posX, int posY, List<WarehouseOrderItem> items )
    {
        OrderId = orderId;
        OrderGroupId = orderGroupId;
        ShippingRoute = shippingRoute;
        CustomerId = customerId;
        DateCreated = dateCreated;
        PosX = posX;
        PosY = posY;
        DateReceived = DateTime.Now;
        Items = items;
    }
    public Guid OrderId { get; private set; }
    public Guid OrderGroupId { get; private set; }
    public ShippingRoute ShippingRoute { get; private set; }
    public string? CustomerId { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime DateReceived { get; private set; }
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    public Pallet Pallet { get; private set; } = Pallet.Empty();
    public List<WarehouseOrderItem> Items { get; private set; }
}