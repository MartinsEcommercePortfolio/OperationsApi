using OperationsDomain.Domain.Catalog;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Models;

public sealed class PickingLine
{
    public Guid Id { get; set; }
    public Product Product { get; set; } = default!;
    public Racking Racking { get; set; } = default!;
    public List<Item> PickedItems { get; set; } = [];
    public int Quantity { get; set; }
    public bool Completed { get; set; }

    public bool ConfirmPickLocation( Guid palletId, Guid rackingId ) =>
        Racking.Id == rackingId && Racking.Pallet is not null && Racking.Pallet.Id == palletId;
    public bool StartPicking( Employee employee )
    {
        if (!Racking.CanBePickedFrom())
            return false;
        
        Racking.AssignTo( employee );
        return true;
    }
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
    public bool Complete( Employee employee )
    {
        if (PickedItems.Count != Quantity)
            return false;
        Completed = true;
        Racking.Free( employee );
        return true;
    }
    public bool IsComplete() =>
        Completed && PickedItems.Count >= Quantity;
}