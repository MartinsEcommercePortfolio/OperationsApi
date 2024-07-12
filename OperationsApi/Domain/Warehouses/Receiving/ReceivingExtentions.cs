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
        var task = employee.GetTask<ReceivingTask>();
        var trailer = task.Trailer;
        var pallet = trailer.CheckPallet( palletId );

        bool received = pallet is not null
            && !w.Pallets.Contains( pallet )
            && trailer.UnloadPallet( pallet );

        if (!received)
            return false;
        
        w.Pallets.Add( pallet! );
        return true;
    }
    public static bool StagePallet( this Warehouse w, Employee employee, Guid palletId, Guid areaId )
    {
        var pallet = w.GetPalletById( palletId );

        if (pallet is null)
            return false;
        
        var task = employee.GetTask<ReceivingTask>();
        var area = task.Area;

        return area.Id == areaId 
            && area.StagePallet( pallet );
    }
}