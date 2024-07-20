using OperationsDomain.Outbound.Shipping.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Employees.Models;

public class Employee
{
    protected Employee( Guid id, string name, Pallet? palletEquipped, WarehouseTask? task )
    {
        Id = id;
        Name = name;
        PalletEquipped = palletEquipped;
        Task = task;
    }

    public static Employee Null() =>
        new( Guid.Empty, string.Empty, null, null );
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid? PalletEquippedId { get; private set; }
    public Pallet? PalletEquipped { get; private set; }
    public WarehouseTask? Task { get; private set; }

    public T? TaskAs<T>() where T : WarehouseTask =>
        Task as T;
    
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