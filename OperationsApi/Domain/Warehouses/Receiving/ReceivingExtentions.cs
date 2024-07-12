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
    public static bool ReceivePallet( this Warehouse w, Employee employee, Pallet pallet )
    {
        bool received = employee
            .GetTask<ReceivingTask>()
            .ReceivePallet( employee, pallet );

        if (received)
            w.Pallets.Add( pallet );

        return received;
    }
    public static bool StagePallet( this Warehouse w, Employee employee, Guid palletId, Guid areaId )
    {
        var pallet = w.GetPalletById( palletId );
        var area = w.GetAreaById( areaId );

        if (pallet is null || area is null)
            return false;

        return employee
            .GetTask<ReceivingTask>()
            .StagePallet( employee, pallet, area );
    }
}