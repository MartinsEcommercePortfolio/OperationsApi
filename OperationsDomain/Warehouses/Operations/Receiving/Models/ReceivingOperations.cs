using OperationsDomain.Warehouses.Employees;
using OperationsDomain.Warehouses.Infrastructure;

namespace OperationsDomain.Warehouses.Operations.Receiving.Models;

public sealed class ReceivingOperations
{
    public Guid Id { get; set; }
    public List<Area> Areas { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<ReceivingTask> PendingReceivingTasks { get; set; } = [];
    public List<ReceivingTask> ActiveReceivingTasks { get; set; } = [];

    public ReceivingTask? GetNextReceivingTask() =>
        PendingReceivingTasks.FirstOrDefault();
    public ReceivingTask? StartReceivingTask( Employee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        var task = PendingReceivingTasks
            .FirstOrDefault( t => t.Id == taskId );

        var stagingArea = GetReceivingArea( areaId );

        var taskStarted = task is not null
            && stagingArea is not null
            && task.Start( employee )
            && task.InitializeStagingArea( trailerId, dockId, stagingArea )
            && task.IsStarted
            && PendingReceivingTasks.Remove( task );

        if (taskStarted)
            ActiveReceivingTasks.Add( task! );

        return task;
    }
    public bool CompleteReceivingTask( Employee employee )
    {
        var receivingTask = employee
            .GetTask<ReceivingTask>();

        if (!receivingTask.IsFinished() || !ActiveReceivingTasks.Remove( receivingTask ))
            return false;
        
        employee.FinishTask();
        return true;
    }

    Area? GetReceivingArea( Guid areaId )
    {
        var area = Areas.FirstOrDefault( a => a.Id == areaId );
        return area is not null && area.CanUse()
            ? area
            : null;
    }
}