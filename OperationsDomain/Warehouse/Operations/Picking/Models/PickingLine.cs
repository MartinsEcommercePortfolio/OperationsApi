using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingLine
{
    public Guid Id { get; private set; }
    public Racking Racking { get; private set; } = default!;
    public List<Item> PickedItems { get; private set; } = [];
    public int Quantity { get; private set; }
    public bool Completed { get; private set; }
    public bool Started { get; set; }
    
    internal bool StartPicking( Employee employee, Guid palletId, Guid rackingId )
    {
        if (Started)
            return false;

        bool started = !Completed
            && !Started
            && Racking.Id == rackingId
            && Racking.Pallet is not null
            && Racking.Pallet.Id == palletId
            && Racking.AssignTo( employee );
        
        return started;
    }
    internal bool PickItem( Employee employee, Guid itemId )
    {
        Item? item = null;
        var pallet = Racking.Pallet;

        bool picked = employee == Racking.Owner
            && pallet is not null
            && PickedItems.All( i => i.Id != itemId )
            && pallet.PickFrom( Racking, employee, itemId, out item );
        
        if (picked)
            PickedItems.Add( item! );

        if (PickedItems.Count == Quantity)
            Completed = true;
        
        return picked;
    }
    internal bool IsComplete() =>
        Completed && PickedItems.Count >= Quantity;
}