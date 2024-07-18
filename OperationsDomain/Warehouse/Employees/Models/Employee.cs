using OperationsDomain.Warehouse.Infrastructure.Units;
using OperationsDomain.Warehouse.Operations;

namespace OperationsDomain.Warehouse.Employees.Models;

public class Employee
{
    public Guid Id { get; private set; }
    public Guid? PalletEquippedId { get; private set; }
    public Pallet? PalletEquipped { get; private set; }
    public WarehouseTask? Task { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public EmployeeWorkMode WorkMode { get; private set; }

    public T? TaskAs<T>() where T : WarehouseTask =>
        Task as T;

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
    
    // ACTIONS
    public bool PickupPallet( Pallet pallet )
    {
        return PalletEquipped is null
            && pallet.Pickup( this );
    }
    public bool DropPallet( Pallet pallet )
    {
        if (PalletEquipped != pallet)
            return false;

        PalletEquipped = null;
        return true;
    }
    public bool LoadPallet( Trailer trailer, Pallet pallet ) =>
        DropPallet( pallet ) &&
        trailer.AddPallet( this, pallet );
    public bool UnloadPallet( Trailer trailer, Pallet pallet ) =>
        trailer.RemovePallet( this, pallet ) &&
        PickupPallet( pallet );
    public bool StagePallet( Area area, Pallet pallet ) =>
        DropPallet( pallet ) &&
        area.AddPallet( this, pallet );
    public bool UnStagePallet( Area area, Pallet pallet ) =>
        area.RemovePallet( this, pallet ) &&
        PickupPallet( pallet );
    public bool RackPallet( Racking racking, Pallet pallet ) =>
        DropPallet( pallet ) &&
        racking.AddPallet( this, pallet );
    public bool UnRackPallet( Racking racking, Pallet pallet ) =>
        racking.RemovePallet( this, pallet ) &&
        PickupPallet( pallet );
}