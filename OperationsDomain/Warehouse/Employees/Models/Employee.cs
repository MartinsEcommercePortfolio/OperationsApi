using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Operations;
using OperationsDomain.Warehouse.Operations.Loading.Models;
using OperationsDomain.Warehouse.Operations.Picking.Models;
using OperationsDomain.Warehouse.Operations.Putaways.Models;
using OperationsDomain.Warehouse.Operations.Receiving.Models;
using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Employees.Models;

public sealed class Employee
{
    public Guid Id { get; private set; }
    public Guid? PalletId { get; private set; }
    public Pallet? Pallet { get; private set; }
    public WarehouseTask? Task { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public EmployeeWorkMode WorkMode { get; private set; }

    public T TaskAs<T>() where T : WarehouseTask, new() => 
        Task as T ?? WarehouseTask.Null<T>();

    public static Employee Null() =>
        new() {
            Id = Guid.Empty,
            Task = null,
            Name = string.Empty,
            WorkMode = EmployeeWorkMode.None
        };
    
    public bool HasTask<T>( out T task ) where T : WarehouseTask, new()
    {
        T? t = Task as T;
        task = t ?? WarehouseTask.Null<T>(); 
        return t is not null;
    }
    public bool StartTask( WarehouseTask task )
    {
        if (Task is not null)
            return false;
        
        Task = task;
        task.StartWith( this );
        return true;
    }
    public bool EndTask()
    {
        if (Task is null || !Task.IsFinished)
            return false;
        
        Task = null;
        return true;
    }
    
    // RECEIVING
    public bool StartReceiving( ReceivingOperations receiving, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        if (Task is not null)
            return false;

        var receivingTask = receiving.GetReceivingTask( taskId );

        return receivingTask is not null
            && receivingTask.InitializeReceiving( trailerId, dockId, areaId )
            && StartTask( receivingTask )
            && receiving.ActivateReceivingTask( receivingTask );
    }
    public bool StartReceivingPallet( Guid trailerId, Guid palletId )
    {
        var task = TaskAs<ReceivingTask>();
        var pallet = task.StartReceivingPallet( trailerId, palletId );

        return pallet is not null
            && UnloadPallet( task.Trailer, pallet );
    }
    public bool FinishReceivingPallet( Guid areaId, Guid palletId )
    {
        var task = TaskAs<ReceivingTask>();

        return task.CurrentPallet is not null
            && StagePallet( task.Area, task.CurrentPallet )
            && task.FinishReceivingPallet( areaId, palletId );
    }
    public bool FinishReceiving( ReceivingOperations receiving )
    {
        var task = TaskAs<ReceivingTask>();

        return EndTask()
            && receiving.CompleteTask( task );
    }
    
    // PUTAWAYS
    public async Task<bool> StartPutaway( PutawayOperations putaways, Guid palletId )
    {
        if (Task is not null)
            return false;

        var putawayTask = await putaways.GenerateTask( palletId );

        return putawayTask is not null
            && StartTask( putawayTask )
            && putaways.AcceptPutawayTask( putawayTask )
            && UnStagePallet( putawayTask.PickupArea, putawayTask.Pallet );
    }
    public bool FinishPutaway( PutawayOperations putaways, Guid rackingId, Guid palletId )
    {
        var task = TaskAs<PutawayTask>();

        return task.CompletePutaway( rackingId, palletId )
            && RackPallet( task.PutawayRacking, task.Pallet )
            && putaways.FinishPutawayTask( task );
    }
    
    // REPLENS
    public bool StartReplenishing( ReplenishingOperations replenishing, Guid taskId, Guid rackingId, Guid palletId )
    {
        if (Task is not null)
            return false;
        
        var task = replenishing.GetPendingTask( taskId );

        return task is not null
            && StartTask( task )
            && replenishing.ActivateTask( task )
            && task.ConfirmPickup( rackingId, palletId )
            && UnRackPallet( task.FromRacking, task.ReplenPallet );
    }
    public bool FinishReplenishing( ReplenishingOperations replenishing, Guid rackingId, Guid palletId )
    {
        var task = TaskAs<ReplenishingTask>();

        return task.ConfirmDeposit( rackingId, palletId )
            && RackPallet( task.ToRacking, task.ReplenPallet )
            && replenishing.RemovedCompletedTask( task )
            && EndTask();
    }
    
    // PICKING
    public bool StartPicking( PickingOperations picking, Guid taskId )
    {
        if (Task is not null)
            return false;

        var pickingTask = picking.GetPendingTask( taskId );

        return pickingTask is not null
            && StartTask( pickingTask )
            && picking.ActivateTask( pickingTask );
    }
    public bool StartPickingLine( Guid lineId, Guid rackingId )
    {
        var task = TaskAs<PickingTask>();
        var pickLine = task.SetPickingLine( lineId );

        return pickLine is not null
            && pickLine.StartPicking( this, rackingId );
    }
    public bool PickItem( PickingOperations picking, Guid itemId )
    {
        var task = TaskAs<PickingTask>();

        return task.CurrentPickLine is not null
            && task.CurrentPickLine.PickItem( this, picking, itemId );
    }
    public bool FinishPickingLine( Guid lineId )
    {
        var task = TaskAs<PickingTask>();
        
        return task.FinishPickingLocation( lineId );
    }
    public bool FinishPicking( PickingOperations picking, Guid areaId )
    {
        var task = TaskAs<PickingTask>();

        return task.InitializeStaging( areaId )
            && StagePallet( task.StagingArea, task.Pallet )
            && picking.RemoveCompletedTask( task )
            && EndTask();
    }
    
    // LOADING
    public bool StartLoading( LoadingOperations loading, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        if (Task is not null)
            return false;

        var loadingTask = loading.GetTask( taskId );

        return loadingTask is not null
            && loadingTask.InitializeLoadingTask( trailerId, dockId, areaId )
            && StartTask( loadingTask )
            && loading.AcceptTask( loadingTask );
    }
    public bool StartLoadingPallet( Guid areaId, Guid palletId )
    {
        var task = TaskAs<LoadingTask>();
        var pallet = task.GetLoadingPallet( areaId, palletId );

        return pallet?.Area != null
            && UnStagePallet( pallet.Area, pallet );
    }
    public bool FinishLoadingPallet( Guid trailerId, Guid palletId )
    {
        var task = TaskAs<LoadingTask>();
        
        return Pallet is not null
            && task.FinishLoadingPallet( trailerId, palletId )
            && LoadPallet( task.TrailerToLoad, Pallet );
    }
    public bool FinishLoading( LoadingOperations loading )
    {
        var task = TaskAs<LoadingTask>();

        return EndTask()
            && loading.CompleteTask( task );
    }
    
    // ACTIONS
    public bool TakePallet( Pallet pallet )
    {
        bool assigned = Pallet is not null
            && pallet.AssignTo( this );

        if (assigned)
            Pallet = pallet;

        return assigned;
    }
    public bool ReleasePallet( Pallet pallet )
    {
        if (Pallet != pallet)
            return false;

        Pallet = null;
        return true;
    }
    public bool LoadPallet( Trailer trailer, Pallet pallet ) =>
        ReleasePallet( pallet ) &&
        trailer.AddPallet( pallet );
    public bool UnloadPallet( Trailer trailer, Pallet pallet ) =>
        trailer.RemovePallet( pallet ) &&
        TakePallet( pallet );
    public bool StagePallet( Area area, Pallet pallet ) =>
        ReleasePallet( pallet ) &&
        area.AddPallet( pallet );
    public bool UnStagePallet( Area area, Pallet pallet ) =>
        area.RemovePallet( pallet ) &&
        TakePallet( pallet );
    public bool RackPallet( Racking racking, Pallet pallet ) =>
        ReleasePallet( pallet ) &&
        racking.AddPallet( pallet );
    public bool UnRackPallet( Racking racking, Pallet pallet ) =>
        racking.RemovePallet() &&
        TakePallet( pallet );
}