using OperationsDomain.Domain.Catalog;
using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingLine
{
    public Guid Id { get; private set; }
    public Product Product { get; private set; } = default!;
    public Racking Racking { get; private set; } = default!;
    public List<Item> PickedItems { get; private set; } = [];
    public int Quantity { get; private set; }
    public bool Completed { get; private set; }

    internal bool ConfirmPickLocation( Guid palletId, Guid rackingId ) =>
        Racking.Id == rackingId && Racking.Pallet is not null && Racking.Pallet.Id == palletId;
    internal bool StartPicking( Employee employee )
    {
        if (!Racking.CanBePickedFrom())
            return false;
        
        Racking.AssignTo( employee );
        return true;
    }
    internal bool PickItem( Employee employee, Guid itemId )
    {
        Item? item = null;
        var pallet = Racking.Pallet;

        bool picked = pallet is not null
            && PickedItems.All( i => i.Id != itemId )
            && pallet.PickFrom( employee, itemId, out item );
        
        if (picked)
            PickedItems.Add( item! );
        
        return picked;
    }
    internal bool IsComplete() =>
        Completed && PickedItems.Count >= Quantity;
}