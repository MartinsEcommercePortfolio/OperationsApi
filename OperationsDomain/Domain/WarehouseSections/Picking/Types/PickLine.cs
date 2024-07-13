using OperationsDomain.Domain.Catalog;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public sealed class PickLine
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid RackingId { get; set; }
    public Product Item { get; set; } = default!;
    public Racking Racking { get; set; } = default!;
    public int Quantity { get; set; }
    public int ItemsLeft { get; set; }

    public bool PickItem( Item item )
    {
        var pallet = Racking.Pallet;
        
        bool picked = pallet is not null
            && pallet.PickItem( item )
            && item.Pick();
        
        if (picked)
            ItemsLeft--;
        
        return picked;
    }
}