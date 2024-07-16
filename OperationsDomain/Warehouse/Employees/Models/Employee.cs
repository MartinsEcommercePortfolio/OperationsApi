using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Operations;
using OperationsDomain.Warehouse.Operations.Loading.Models;
using OperationsDomain.Warehouse.Operations.Picking.Models;

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

    public bool StartLoadingTask( LoadingTask loadingTask, Guid trailerId, Guid dockId, Guid areaId ) =>
        loadingTask.StartWith( this ) &&
        loadingTask.InitializeLoadingTask( trailerId, dockId, areaId ) &&
        StartTask( loadingTask );
    public bool StartLoadingPallet( Guid areaId, Guid palletId ) =>
        TaskAs<LoadingTask>().StartLoadingPallet( areaId, palletId );
    public bool FinishLoadingPallet( Guid trailerId, Guid palletId ) =>
        TaskAs<LoadingTask>().FinishLoadingPallet( trailerId, palletId );
}