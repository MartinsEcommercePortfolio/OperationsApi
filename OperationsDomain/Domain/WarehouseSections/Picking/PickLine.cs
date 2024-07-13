using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking;

public sealed class PickLine
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid RackingId { get; set; }
    public Item Item { get; set; } = default!;
    public Racking Racking { get; set; } = default!;
    public int Quantity { get; set; }
}