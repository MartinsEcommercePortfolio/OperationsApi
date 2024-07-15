using OperationsDomain.Warehouse.Employees;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishingOperations
{
    public Guid Id { get; set; }
    public List<ReplenishingTask> PendingReplenishingTasks { get; set; } = [];
    public List<ReplenishingTask> ActiveReplenishingTasks { get; set; } = [];

    public ReplenishingTask? GetNextReplenishingTask()
    {
        var task = PendingReplenishingTasks.FirstOrDefault();
        return task;
    }
    public ReplenishingTask? StartReplenishingTask( Employee employee, Guid taskId )
    {
        var task = PendingReplenishingTasks.FirstOrDefault( t => t.Id == taskId );

        bool started = task is not null
            && !PendingReplenishingTasks.Contains( task )
            && task.Start( employee )
            && PendingReplenishingTasks.Remove( task );

        if (!started)
            return null;
        
        ActiveReplenishingTasks.Add( task! );
        return task;
    }
    public bool ReplenishLocation( Employee employee, Guid palletId, Guid rackingId )
    {
        var task = employee
            .GetTask<ReplenishingTask>();
        
        var replenished = task
            .ReplenishLocation( palletId, rackingId );

        if (!replenished)
            return false;

        return replenished
            && ActiveReplenishingTasks.Remove( task )
            && employee.FinishTask();
    }
}