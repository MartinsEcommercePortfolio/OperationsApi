using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; private set; }
    public List<PickingTask> PendingPickingTasks { get; private set; } = [];
    public List<PickingTask> ActivePickingTasks { get; private set; } = [];

    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    public PickingTask? StartPickingTask( Employee employee, Guid taskId )
    {
        var pickingTask = PendingPickingTasks
            .FirstOrDefault( t => t.Id == taskId );

        bool started = pickingTask is not null 
            && !ActivePickingTasks.Contains( pickingTask )
            && employee.StartPicking( pickingTask )
            && PendingPickingTasks.Remove( pickingTask );

        if (started)
            ActivePickingTasks.Add( pickingTask! );
        
        return pickingTask;
    }
    public bool FinishPickingTask( Employee employee, Guid areaId )
    {
        var task = employee
            .TaskAs<PickingTask>();

        return employee.StagePick( areaId )
            && employee.EndTask()
            && ActivePickingTasks.Remove( task );
    }
}