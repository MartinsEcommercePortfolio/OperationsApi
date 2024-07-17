using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingLine
{
    public Guid Id { get; private set; }
    public Racking Racking { get; private set; } = default!;
    public List<Item> PickedItems { get; private set; } = [];
    public int Quantity { get; private set; }
    public bool Completed { get; private set; }
    public bool Started { get; private set; }
    
    internal bool StartPicking( Employee employee, Guid rackingId )
    {
        if (Started || Completed)
            return false;

        bool started = Racking.Id == rackingId
            && Racking.AssignTo( employee );
        
        return started;
    }
    internal bool PickItem( Employee employee, ReplenishingOperations replenishing, Guid itemId )
    {
        Item? item = null;
        var pallet = Racking.Pallet;

        var picked = pallet is not null 
            && employee == Racking.Owner
            && PickedItems.All( i => i.Id != itemId )
            && pallet.PickFrom( Racking, employee, itemId, out item );

        if (!picked)
            return false;
        
        PickedItems.Add( item! );

        if (pallet!.IsEmpty())
            replenishing.SubmitReplenishEvent( new ReplenishEvent( Racking ) );

        if (PickedItems.Count == Quantity)
            Completed = true;
        
        return picked;
    }
    internal bool IsComplete() =>
        Completed && PickedItems.Count >= Quantity;
}