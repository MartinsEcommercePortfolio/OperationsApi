using OperationsDomain.Domain.Catalog;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public sealed class PickingLine
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid RackingId { get; set; }
    public Product Product { get; set; } = default!;
    public Racking Racking { get; set; } = default!;
    public List<Item> PickedItems { get; set; } = [];
    public int Quantity { get; set; }

    public int? ConfirmPickLocation( Guid rackingId ) =>
        Racking.Id == rackingId
            ? Quantity
            : null;
    public int ItemsRemainingInPick() =>
        Quantity - PickedItems.Count;
    public bool PickItem( Employee employee, Guid itemId )
    {
        Item? item = null;
        var pallet = Racking.Pallet;

        bool picked = pallet is not null
            && PickedItems.All( i => i.Id != itemId )
            && pallet.PickItem( employee, itemId, out item );
        
        if (picked)
            PickedItems.Add( item! );
        
        return picked;
    }
    public bool IsComplete() =>
        PickedItems.Count >= Quantity;
}