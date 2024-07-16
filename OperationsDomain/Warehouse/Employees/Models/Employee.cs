using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Operations;
using OperationsDomain.Warehouse.Operations.Loading.Models;
using OperationsDomain.Warehouse.Operations.Picking.Models;
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
    
    public bool UnloadPallet( Trailer trailer, Pallet pallet ) =>
        trailer.RemovePallet( pallet ) &&
        TakePallet( pallet );
    public bool StagePallet( Area area, Pallet pallet ) =>
        Pallet == pallet &&
        area.AddPallet( Pallet ) &&
        ReleasePallet();
    public bool UnStagePallet( Area area, Pallet pallet ) =>
        area.RemovePallet( pallet ) &&
        TakePallet( pallet );
    public bool RackPallet( Racking racking, Pallet pallet ) =>
        ReleasePallet() &&
        racking.AddPallet( pallet );
    public bool UnRackPallet( Racking racking, Pallet pallet ) =>
        racking.RemovePallet() &&
        TakePallet( pallet );

    public bool StartPicking( PickingTask pickingTask ) =>
        StartTask( pickingTask ) &&
        pickingTask.StartWith( this );
    public bool StartPickingLocation( Guid rackingId, Guid palletId ) =>
        TaskAs<PickingTask>().StartPickingLocation( rackingId, palletId );
    public bool PickItem( Guid itemId ) =>
        TaskAs<PickingTask>().PickItem( itemId );
    public bool FinishPickingLocation( Guid rackingId, Guid palletId ) =>
        TaskAs<PickingTask>().FinishPickingLocation( rackingId, palletId );
    public bool FinishPicking( Guid areaId ) =>
        TaskAs<PickingTask>().StagePick( areaId ) &&
        Task!.Finish( this ) &&
        EndTask();

    public bool StartReplenishing( ReplenishingTask replenishingTask ) =>
        replenishingTask.StartWith( this ) &&
        StartTask( replenishingTask );
    public bool PickupReplenishment( Guid palletId ) =>
        TaskAs<ReplenishingTask>().PickupReplenishingPallet( palletId );
    public bool ReplenishLocation( Guid rackingId, Guid palletId ) =>
        TaskAs<ReplenishingTask>().ReplenishLocation( rackingId, palletId ) &&
        Task!.Finish( this ) &&
        EndTask();

    public bool StartLoadingTask( LoadingTask loadingTask, Guid trailerId, Guid dockId, Guid areaId ) =>
        loadingTask.StartWith( this ) &&
        loadingTask.InitializeLoadingTask( trailerId, dockId, areaId ) &&
        StartTask( loadingTask );
    public bool StartLoadingPallet( Guid areaId, Guid palletId ) =>
        TaskAs<LoadingTask>().StartLoadingPallet( areaId, palletId );
    public bool FinishLoadingPallet( Guid trailerId, Guid palletId ) =>
        TaskAs<LoadingTask>().FinishLoadingPallet( trailerId, palletId );

    public bool TakePallet( Pallet pallet )
    {
        bool assigned = Pallet is not null
            && pallet.AssignTo( this );
        
        if (assigned)
            Pallet = pallet;
        
        return assigned;
    }
    public bool ReleasePallet()
    {
        if (Pallet is null)
            return false;
        
        Pallet = null;
        return true;
    }
}