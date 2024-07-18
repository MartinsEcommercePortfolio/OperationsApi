using OperationsDomain.Warehouse.Infrastructure.Units;
using OperationsDomain.Warehouse.Operations;

namespace OperationsDomain.Warehouse.Employees.Models;

public class Employee
{
    public Employee( string name )
    {
        Name = name;
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid? PalletEquippedId { get; private set; }
    public Pallet? PalletEquipped { get; private set; }
    public WarehouseTask? Task { get; private set; }

    public T? TaskAs<T>() where T : WarehouseTask =>
        Task as T;

    public static Employee Null() =>
        new( string.Empty );
    
    public bool StartTask( WarehouseTask task )
    {
        if (Task is not null)
            return false;
        
        Task = task;
        return Task.StartWith( this );
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