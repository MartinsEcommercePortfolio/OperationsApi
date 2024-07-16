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
    public bool StartReceiving( ReceivingTask task, Guid trailerId, Guid dockId, Guid areaId ) =>
        StartTask( task ) &&
        task.InitializeReceiving( trailerId, dockId, areaId );
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
    
    // PUTAWAYS
    public bool StartPutaway( PutawayTask task, Racking racking, Pallet pallet ) =>
        pallet.Area is not null &&
        StartTask( task ) &&
        task.InitializePutaway( racking, pallet ) &&
        UnStagePallet( pallet.Area, pallet );
    public bool FinishPutaway( Guid rackingId, Guid palletId )
    {
        var task = TaskAs<PutawayTask>();

        return task.CompletePutaway( rackingId, palletId )
            && RackPallet( task.PutawayRacking, task.Pallet );
    }
    
    // REPLENS
    public bool StartReplenishing( ReplenishingTask task ) =>
        StartTask( task );
    public bool PickReplenishingPallet( Guid rackingId, Guid palletId )
    {
        var task = TaskAs<ReplenishingTask>();

        return task.ConfirmPickup( rackingId, palletId )
            && UnRackPallet( task.FromRacking, task.ReplenPallet );
    }
    public bool ReplenishLocation( Guid rackingId, Guid palletId )
    {
        var task = TaskAs<ReplenishingTask>();

        return task.ConfirmDeposit( rackingId, palletId )
            && RackPallet( task.ToRacking, task.ReplenPallet );
    }
    
    // PICKING
    public bool StartPicking( PickingTask task ) =>
        StartTask( task );
    public bool StartPickingLine( Guid lineId, Guid rackingId, Guid palletId )
    {
        var task = TaskAs<PickingTask>();
        var pickLine = task.SetPickingLine( lineId );

        return pickLine is not null
            && pickLine.StartPicking( this, rackingId, palletId );
    }
    public bool PickItem( Guid itemId )
    {
        var task = TaskAs<PickingTask>();

        return task.CurrentPickLine is not null
            && task.CurrentPickLine.PickItem( this, itemId );
    }
    public bool FinishPickingLine( Guid lineId )
    {
        var task = TaskAs<PickingTask>();
        
        return task.FinishPickingLocation( lineId );
    }
    public bool StagePick( Guid areaId )
    {
        var task = TaskAs<PickingTask>();
        
        return StagePallet( task.StagingArea, task.Pallet )
            && task.StagePick( areaId );
    }
    
    // LOADING
    public bool StartLoadingTask( LoadingTask loadingTask, Guid trailerId, Guid dockId, Guid areaId ) =>
        loadingTask.InitializeLoadingTask( trailerId, dockId, areaId ) &&
        StartTask( loadingTask );
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