using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; set; }
    public List<Pallet> Pallets { get; set; } = [];
    public List<PickingTask> PendingPickingTasks { get; set; } = [];
    public List<PickingTask> ActivePickingTasks { get; set; } = [];

    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    public PickingTask? StartPickingTask( Employee employee, Guid taskId )
    {
        var task = PendingPickingTasks
            .FirstOrDefault( t => t.Id == taskId );
        
        if (task is null)
            return null;

        bool started = !ActivePickingTasks.Contains( task )
            && task.Start( employee )
            && !Pallets.Contains( task.Pallet )
            && PendingPickingTasks.Remove( task );

        if (!started)
            return null;
        
        ActivePickingTasks.Add( task );
        Pallets.Add( task.Pallet );
        return task;
    }
    public bool StageAndFinishPickingOrder( Employee employee, Guid areaId )
    {
        var task = employee.GetTask<PickingTask>();
        var staged = task.StagePick( areaId );
        return staged
            && task.IsCompleted
            && ActivePickingTasks.Remove( task );
    }
}