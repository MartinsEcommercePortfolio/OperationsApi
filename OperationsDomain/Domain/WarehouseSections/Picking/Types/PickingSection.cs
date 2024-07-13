using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public sealed class PickingSection
{
    public Guid Id { get; set; }
    public List<Item> Items { get; set; } = [];
    public List<PickingTask> PendingPickingTasks { get; set; } = [];
    public List<PickingTask> ActivePickingTasks { get; set; } = [];

    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    public bool BeginPickingTask( Employee employee, Guid taskId )
    {
        var task = PendingPickingTasks.FirstOrDefault( t => t.Id == taskId );
        if (task is null)
            return false;

        if (!task.Start( employee ))
            return false;
        
        PendingPickingTasks.Remove( task );
        ActivePickingTasks.Add( task );
        return true;
    }
    public Racking? GetNextPickLocation( Employee employee ) =>
        employee
            .GetTask<PickingTask>()
            .GetNextPickLocation();
    public bool ConfirmPickLocation( Employee employee, Guid rackingId ) =>
        employee
            .GetTask<PickingTask>()
            .ConfirmPickLocation( rackingId );
    public int? PickItem( Employee employee, Guid itemId )
    {
        var item = Items.FirstOrDefault( i => i.Id == itemId );
        if (item is null)
            return null;

        var task = employee
            .GetTask<PickingTask>();
        
        var itemsLeft = task
            .PickItem( item );

        if (task.IsCompleted)
            ActivePickingTasks.Remove( task );

        return itemsLeft;
    }
}