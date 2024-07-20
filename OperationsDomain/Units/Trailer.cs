using OperationsDomain.Employees.Models;

namespace OperationsDomain.Units;

public sealed class Trailer : Unit
{
    Trailer( Guid id, int number, Employee? employee, TrailerState state, Dock? dock ) 
        : base( id, number, employee )
    {
        Id = Guid.NewGuid();
        State = state;
        Dock = dock;
    }

    public static Trailer New( int number ) =>
        new( Guid.NewGuid(), number, null, TrailerState.Parked, null );

    public TrailerState State { get; private set; } 
    public Dock? Dock { get; private set; }

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
        var docked = State != TrailerState.Docked
            && Dock is null;

        if (!docked)
            return false;
        
        Dock = dock;
        State = TrailerState.Docked;

        return true;
    }
    public bool UnDock( Dock dock )
    {
        var undocked = State == TrailerState.Docked
            && Dock == dock;

        if (!undocked)
            return false;
        
        Dock = null;
        State = TrailerState.Travelling;
        
        return true;
    }
}