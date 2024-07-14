using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Receiving.Types;

public sealed class ReceivingSection
{
    public Guid Id { get; set; }
    public List<Area> Areas { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<ReceivingTask> PendingReceivingTasks { get; set; } = [];
    public List<ReceivingTask> ActiveReceivingTasks { get; set; } = [];

    public ReceivingTask? GetNextReceivingTask() =>
        PendingReceivingTasks.FirstOrDefault();
    public bool StartReceivingTask( Employee employee, Guid taskId )
    {
        var task = PendingReceivingTasks
            .FirstOrDefault( t => t.Id == taskId );

        bool taskStarted = task is not null
            && task.Start( employee )
            && PendingReceivingTasks.Remove( task );

        if (taskStarted)
            ActiveReceivingTasks.Add( task! );
        
        return taskStarted;
    }
    public bool ReceiveUnloadedPallet( Employee employee, Guid palletId )
    {
        var pallet = employee
            .GetTask<ReceivingTask>()
            .ReceivePallet( palletId );

        bool received = pallet is not null &&
            !Pallets.Contains( pallet );

        if (received)
            Pallets.Add( pallet! );
        
        return received;
    }
    public bool CompleteReceivingTask( Employee employee )
    {
        var receivingTask = employee
            .GetTask<ReceivingTask>();
        
        if (!receivingTask.IsFinished()) 
            return false;
        
        ActiveReceivingTasks.Remove( receivingTask );
        employee.FinishTask();

        return true;
    }
}