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
    public Guid DeviceId { get; private set; }
    public Guid ForkliftId { get; private set; }
    public Guid TaskId { get; private set; }
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
            DeviceId = Guid.Empty,
            ForkliftId = Guid.Empty,
            TaskId = Guid.Empty,
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
        
        TaskId = task.Id;
        Task = task;
        task.StartWith( this );
        return true;
    }
    public bool EndTask()
    {
        if (Task is null)
            return false;
        
        TaskId = Guid.Empty;
        Task = null;
        return true;
    }

    public bool StartReceivingTask( ReceivingTask receivingTask, Guid trailerId, Guid dockId, Area stagingArea ) =>
        StartTask( receivingTask ) &&
        receivingTask.StartWith( this ) &&
        receivingTask.InitializeStagingArea( trailerId, dockId, stagingArea );
    public bool UnloadPallet( Guid trailerId, Guid palletId ) =>
        PalletId is null && Pallet is null &&
        TaskAs<ReceivingTask>().StartReceivingPallet( trailerId, palletId );
    public bool StagePallet( Guid areaId, Guid palletId ) =>
        TaskAs<ReceivingTask>().FinishReceivingPallet( areaId, palletId );

    public bool StartPutawayTask( PutawayTask putawayTask, Racking racking, Pallet pallet ) =>
        putawayTask.InitializeFrom( this, racking, pallet ) && 
        putawayTask.StartWith( this ) && 
        StartTask( putawayTask );
    public bool FinishPutaway( Guid rackingId, Guid palletId ) =>
        TaskAs<PutawayTask>().CompletePutaway( rackingId, palletId ) &&
        EndTask();

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
    public bool FinishLoadingTask() =>
        Task is not null &&
        Task.Finish( this ) &&
        EndTask();
}