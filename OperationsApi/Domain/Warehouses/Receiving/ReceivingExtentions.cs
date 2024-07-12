using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses.Receiving;

internal static class ReceivingExtentions
{
    public static bool StartReceivingTask( this Warehouse w, Employee employee, Guid taskId )
    {
        var task = w.PendingReceivingTasks
            .FirstOrDefault( t => t.Id == taskId );

        if (task is null)
            return false;

        task.Start( employee );
        w.PendingReceivingTasks.Remove( task );
        w.ActiveReceivingTasks.Add( task );
        
        return true;
    }
    public static bool ReceivePallet( this Warehouse w, Employee employee, Guid palletId )
    {
        var pallet = employee
            .GetTask<ReceivingTask>()
            .ReceivePallet( palletId );

        if (pallet is null || w.Pallets.Contains( pallet ))
            return false;
        
        w.Pallets.Add( pallet );
        return true;
    }
    public static bool StagePallet( this Warehouse w, Employee employee, Guid palletId, Guid areaId )
    {
        var task = employee
            .GetTask<ReceivingTask>();

        var staged = task
            .StagePallet( palletId, areaId );

        return staged && 
            w.ActiveReceivingTasks.Remove( task );
    }
}