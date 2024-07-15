using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; private set; }
    public List<Pallet> Pallets { get; private set; } = [];
    public List<PickingTask> PendingPickingTasks { get; private set; } = [];
    public List<PickingTask> ActivePickingTasks { get; private set; } = [];

    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    public PickingTask? StartPickingTask( Employee employee, Guid taskId )
    {
        var pickingTask = PendingPickingTasks
            .FirstOrDefault( t => t.Id == taskId );
        
        if (pickingTask is null)
            return null;

        bool started = !ActivePickingTasks.Contains( pickingTask )
            && !Pallets.Contains( pickingTask.Pallet )
            && employee.StartPicking( pickingTask )
            && PendingPickingTasks.Remove( pickingTask );

        if (!started)
            return null;
        
        ActivePickingTasks.Add( pickingTask );
        Pallets.Add( pickingTask.Pallet );
        return pickingTask;
    }
    public bool StageAndFinishPickingOrder( Employee employee, Guid areaId )
    {
        var task = employee.TaskAs<PickingTask>();

        return employee.FinishPicking( areaId )
            && ActivePickingTasks.Remove( task );
    }
}