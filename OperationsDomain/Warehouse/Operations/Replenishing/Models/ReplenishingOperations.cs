using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishingOperations
{
    public Guid Id { get; set; }
    public List<ReplenishingTask> PendingReplenishingTasks { get; private set; } = [];
    public List<ReplenishingTask> ActiveReplenishingTasks { get; private set; } = [];

    public ReplenishingTask? GetNextReplenishingTask()
    {
        var task = PendingReplenishingTasks.FirstOrDefault();
        return task;
    }
    public ReplenishingTask? StartReplenishingTask( Employee employee, Guid taskId )
    {
        var replenishingTask = PendingReplenishingTasks
            .FirstOrDefault( t => t.Id == taskId );

        var started = replenishingTask is not null
            && employee.StartTask( replenishingTask )
            && PendingReplenishingTasks.Remove( replenishingTask );

        if (started)
            ActiveReplenishingTasks.Add( replenishingTask! );
        
        return replenishingTask;
    }
    public bool FinishReplenishingTask( Employee employee )
    {
        var task = employee.TaskAs<ReplenishingTask>();
        
        return task.IsFinished
            && employee.EndTask()
            && ActiveReplenishingTasks.Remove( task );
    }
}