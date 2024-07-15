using OperationsDomain.Warehouse.Employees;

namespace OperationsDomain.Warehouse.Operations.Loading.Models;

public sealed class LoadingOperations
{
    public Guid Id { get; private set; }
    public List<LoadingTask> PendingLoadingTasks { get; private set; } = [];
    public List<LoadingTask> ActiveLoadingTasks { get; private set; } = [];

    public LoadingTask? GetNextLoadingTask() =>
        PendingLoadingTasks.FirstOrDefault();

    public LoadingTask? StartLoadingTask( Employee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        var loadingTask = PendingLoadingTasks
            .FirstOrDefault( t => t.Id == taskId );

        var taskStarted = loadingTask is not null
            && employee.StartLoadingTask( loadingTask, trailerId, dockId, areaId )
            && PendingLoadingTasks.Remove( loadingTask );
        
        if (taskStarted)
            ActiveLoadingTasks.Add( loadingTask! );
        
        return loadingTask;
    }
    public bool FinishLoadingTask( Employee employee )
    {
        var loadingTask = employee
            .TaskAs<LoadingTask>();

        return employee.FinishLoadingTask()
            && ActiveLoadingTasks.Remove( loadingTask );
    }
}