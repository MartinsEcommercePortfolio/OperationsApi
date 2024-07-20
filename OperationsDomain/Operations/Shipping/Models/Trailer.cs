using OperationsDomain.Employees.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Operations.Shipping.Models;

public sealed class Trailer : Unit
{
    Trailer( Guid id, int number, Employee? employee, TrailerState state, Dock? dock, int palletCapacity, List<Pallet> pallets ) 
        : base( id, number, employee )
    {
        Id = Guid.NewGuid();
        State = state;
        Dock = dock;
        PalletCapacity = palletCapacity;
        Pallets = pallets;
    }

    public static Trailer New( int number, int capacity ) =>
        new( Guid.NewGuid(), number, null, TrailerState.Parked, null, capacity, [] );

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
}