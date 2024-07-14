using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public sealed class PickingSection
{
    public Guid Id { get; set; }
    public List<Pallet> Pallets { get; set; } = [];
    public List<PickingTask> PendingPickingTasks { get; set; } = [];
    public List<PickingTask> ActivePickingTasks { get; set; } = [];

    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    public bool StartPickingTask( Employee employee, Guid taskId )
    {
        var task = PendingPickingTasks
            .FirstOrDefault( t => t.Id == taskId );
        
        if (task is null)
            return false;

        bool started = !ActivePickingTasks.Contains( task )
            && task.Start( employee )
            && !Pallets.Contains( task.Pallet )
            && PendingPickingTasks.Remove( task );

        if (!started)
            return false;
        
        ActivePickingTasks.Add( task );
        Pallets.Add( task.Pallet );
        return true;
    }
    public bool CompletePickingTask( Employee employee, Guid areaId )
    {
        var task = employee.GetTask<PickingTask>();
        var staged = task.StagePick( areaId );
        return staged
            && task.IsCompleted
            && ActivePickingTasks.Remove( task );
    }
}