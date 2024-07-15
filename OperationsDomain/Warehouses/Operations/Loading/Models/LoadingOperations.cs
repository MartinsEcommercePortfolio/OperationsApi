using OperationsDomain.Warehouses.Employees;

namespace OperationsDomain.Warehouses.Operations.Loading.Models;

public sealed class LoadingOperations
{
    public Guid Id { get; set; }
    public List<LoadingTask> PendingLoadingTasks { get; set; } = [];
    public List<LoadingTask> ActiveLoadingTasks { get; set; } = [];

    public LoadingTask? GetNextLoadingTask() =>
        PendingLoadingTasks.FirstOrDefault();

    public LoadingTask? StartLoadingTask( Employee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        var task = PendingLoadingTasks
            .FirstOrDefault( t => t.Id == taskId );

        var taskStarted = task is not null
            && task.Start( employee )
            && task.IsStarted
            && task.InitializeLoadingTask( trailerId, dockId, areaId )
            && PendingLoadingTasks.Remove( task );
        
        if (taskStarted)
            ActiveLoadingTasks.Add( task! );
        
        return task;
    }

    public bool FinishLoadingTask( Employee employee )
    {
        var loadingTask = employee
            .GetTask<LoadingTask>();

        if (!loadingTask.IsCompleted || !ActiveLoadingTasks.Remove( loadingTask ))
            return false;

        employee.FinishTask();
        return true;
    }
}