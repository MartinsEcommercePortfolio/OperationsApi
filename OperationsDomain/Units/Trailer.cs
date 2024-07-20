using OperationsDomain.Employees.Models;

namespace OperationsDomain.Units;

public sealed class Trailer : Unit
{
    Trailer( Guid id, int number, Employee? employee, TrailerStatus status, DateTime lastUpdated, Dock? dock, Guid? warehouseAssignmentId, int capacity, List<Pallet> pallets ) 
        : base( id, number, employee )
    {
        Id = Guid.NewGuid();
        Status = status;
        LastUpdated = lastUpdated;
        Dock = dock;
        WarehouseAssignmentId = warehouseAssignmentId;
        PalletCapacity = capacity;
        Pallets = pallets;
    }

    public static Trailer NewReceiving( int number ) =>
        new( Guid.NewGuid(), number, null, TrailerStatus.Parked, DateTime.Now, null, null, 0, [] );

    public TrailerStatus Status { get; private set; }
    public DateTime LastUpdated { get; private set; }
    
    public Dock? Dock { get; private set; }
    public Guid? WarehouseAssignmentId { get; private set; }

    public int PalletCapacity { get; private set; }
    public List<Pallet> Pallets { get; private set; }
    
    public Pallet? GetPallet( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
    public bool AddPallet( Employee employee, Pallet pallet )
    {
        var added = IsOwnedBy( employee )
            && Pallets.Count < PalletCapacity
            && !Pallets.Contains( pallet );

        if (added)
            Pallets.Add( pallet );

        return true;
    }
    public bool RemovePallet( Employee employee, Pallet pallet )
    {
        return IsOwnedBy( employee )
            && Pallets.Remove( pallet );
    }
    public bool DockTo( Dock dock )
    {
        var docked = Status != TrailerStatus.Docked
            && Dock is null;

        if (!docked)
            return false;
        
        Dock = dock;
        Status = TrailerStatus.Docked;
        LastUpdated = DateTime.Now;

        return true;
    }
    public bool UnDock( Dock dock )
    {
        var undocked = Status == TrailerStatus.Docked
            && Dock == dock;

        if (!undocked)
            return false;
        
        Dock = null;
        Status = TrailerStatus.Travelling;
        LastUpdated = DateTime.Now;
        
        return true;
    }
    public bool AssignTask( Guid id )
    {
        if (WarehouseAssignmentId is not null)
            return false;

        WarehouseAssignmentId = id;

        return true;
    }
    public bool UnAssignTask( Guid id )
    {
        if (WarehouseAssignmentId != id)
            return false;

        WarehouseAssignmentId = null;

        return true;
    }
}