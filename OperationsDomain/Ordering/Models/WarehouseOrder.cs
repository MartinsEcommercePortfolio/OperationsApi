using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Ordering.Models;

public sealed class WarehouseOrder
{
    public WarehouseOrder( Guid orderId, Guid orderGroupId, string? customerId, DateTime dateCreated, int posX, int posY, List<WarehouseOrderItem> items )
    {
        OrderId = orderId;
        OrderGroupId = orderGroupId;
        CustomerId = customerId;
        DateCreated = dateCreated;
        PosX = posX;
        PosY = posY;
        DateReceived = DateTime.Now;
        Items = items;
        Pallets = [];
    }
    
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid OrderGroupId { get; private set; }
    public Guid ShippingRouteId { get; private set; }
    public Guid? ShipmentId { get; private set; }
    public string? CustomerId { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime DateReceived { get; private set; }
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    public List<Pallet> Pallets { get; private set; }
    public List<WarehouseOrderItem> Items { get; private set; }
}