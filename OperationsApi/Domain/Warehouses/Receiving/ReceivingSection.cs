using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses.Receiving;

internal sealed class ReceivingSection
{
    public Guid Id { get; set; }
    public List<Area> Areas { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<ReceivingTask> PendingReceivingTasks { get; set; } = [];
    public List<ReceivingTask> ActiveReceivingTasks { get; set; } = [];
    
    public bool BeginReceivingTask( Employee employee, Guid taskId )
    {
        var task = PendingReceivingTasks
            .FirstOrDefault( t => t.Id == taskId );

        if (task is null)
            return false;

        task.Start( employee );
        PendingReceivingTasks.Remove( task );
        ActiveReceivingTasks.Add( task );
        
        return true;
    }
    public bool ReceivePallet( Employee employee, Guid palletId )
    {
        var pallet = employee
            .GetTask<ReceivingTask>()
            .ReceivePallet( palletId );

        if (pallet is null || Pallets.Contains( pallet ))
            return false;
        
        Pallets.Add( pallet );
        return true;
    }
    public bool StageReceivedPallet( Employee employee, Guid palletId, Guid areaId )
    {
        var receivingTask = employee
            .GetTask<ReceivingTask>();

        var staged = receivingTask
            .StagePallet( palletId, areaId );

        if (receivingTask.IsFinished())
            ActiveReceivingTasks.Remove( receivingTask );
        
        return staged;
    }
}