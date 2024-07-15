using OperationsDomain.Warehouse.Employees;
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
        var task = PendingReplenishingTasks.FirstOrDefault( t => t.Id == taskId );

        var started = task is not null
            && !PendingReplenishingTasks.Contains( task )
            && employee.StartReplenishing( task )
            && PendingReplenishingTasks.Remove( task );

        if (started)
            ActiveReplenishingTasks.Add( task! );
        
        return task;
    }
    public bool FinishReplenishingTask( Employee employee, Guid rackingId, Guid palletId )
    {
        var task = employee.TaskAs<ReplenishingTask>();
        
        return employee.ReplenishLocation( palletId, rackingId )
            && ActiveReplenishingTasks.Remove( task );
    }
}