using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public sealed class PickingTask : WarehouseTask
{
    public PickLine? CurrentPickLine { get; set; }
    public List<PickLine> PickLines { get; set; } = [];
    public List<Item> PickedItems { get; set; } = [];
    
    // get(return: fail or task-summary
    // start(return: fail or success)
    // get next pick line (return: location or pick finished)
    // confirm pick location (return: fail or productIdToPick
    // pick item (return: fail, pick another, pick-line finished)
    
    public Racking? GetNextPickLocation()
    {
        return null;
    }
    public bool ConfirmPickLocation( Guid rackingId )
    {
        return false;
    }
    public int? PickItem( Item item )
    {
        bool picked = CurrentPickLine is not null
            && CurrentPickLine.PickItem( item );
        return picked
            ? CurrentPickLine!.ItemsLeft
            : null;
    }
}