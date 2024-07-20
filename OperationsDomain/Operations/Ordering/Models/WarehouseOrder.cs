using OperationsDomain.Infrastructure.Units;

namespace OperationsDomain.Operations.Ordering.Models;

public sealed class WarehouseOrder
{
    public WarehouseOrder( Guid id, Guid orderId, Guid orderGroupId, string? customerId, OrderStatus status, DateTime dateCreated, DateTime dateUpdated, int posX, int posY, List<WarehouseOrderItem> items )
    {
        Id = id;
        OrderId = orderId;
        OrderGroupId = orderGroupId;
        CustomerId = customerId;
        Status = status;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        PosX = posX;
        PosY = posY;
        DateUpdated = DateTime.Now;
        Items = items;
        Pallets = [];
    }

    public static WarehouseOrder New( Guid orderId, Guid orderGroupId, string? customerId, DateTime dateCreated, int posX, int posY, List<WarehouseOrderItem> items ) =>
        new( Guid.NewGuid(), orderId, orderGroupId, customerId, OrderStatus.Pending, dateCreated, DateTime.Now, posX, posY, items );
    
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid OrderGroupId { get; private set; }
    public Guid? ShipmentId { get; private set; }
    public Guid? TrailerId { get; private set; }
    public string? CustomerId { get; private set; }
    public OrderStatus Status { get; private set; } 
    public DateTime DateCreated { get; private set; }
    public DateTime DateUpdated { get; private set; }
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    public List<Pallet> Pallets { get; private set; }
    public List<WarehouseOrderItem> Items { get; private set; }

    public bool AssignTrailer( Trailer trailer )
    {
        if (TrailerId is not null)
            return false;

        TrailerId = trailer.Id;
        return true;
    }
    public bool Update( OrderStatus newStatus )
    {
        var invalidStatus = newStatus > Status
            || Status == OrderStatus.Delivered
            || Status == OrderStatus.Returned;

        if (invalidStatus)
            return false;

        var invalidShipping = newStatus is OrderStatus.Shipped
            && (ShipmentId is null || TrailerId is null);
        
        if (invalidShipping)
            return false;

        Status = newStatus;
        DateUpdated = DateTime.Now;

        return true;
    }
}