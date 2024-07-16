using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Receiving.Models;

public sealed class ReceivingOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<ReceivingTask> PendingReceivingTasks { get; private set; } = [];
    public List<ReceivingTask> ActiveReceivingTasks { get; private set; } = [];
    
    // Shipping
    public bool ReceiveTrailerWithPallets( string trailerNumber, string dockNumber, List<Pallet> pallets )
    {
        return false;
    }
    
    // Tasks
    public ReceivingTask? GetNextReceivingTask() =>
        PendingReceivingTasks.FirstOrDefault();
    public ReceivingTask? StartReceivingTask( Employee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        var receivingTask = PendingReceivingTasks
            .FirstOrDefault( t => t.Id == taskId );

        var taskStarted = receivingTask is not null
            && receivingTask.InitializeReceiving( trailerId, dockId, areaId )
            && employee.StartTask( receivingTask )
            && PendingReceivingTasks.Remove( receivingTask );

        if (taskStarted)
            ActiveReceivingTasks.Add( receivingTask! );
    
        return receivingTask;
    }
    public bool CompleteReceivingTask( Employee employee )
    {
        var receivingTask = employee
            .TaskAs<ReceivingTask>();
        
        return receivingTask.IsFinished
            && employee.EndTask()
            && ActiveReceivingTasks.Remove( receivingTask );
    }
}