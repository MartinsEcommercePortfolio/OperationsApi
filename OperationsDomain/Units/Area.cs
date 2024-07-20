using OperationsDomain.Employees.Models;

namespace OperationsDomain.Units;

public sealed class Area : Unit
{
    Area( Guid id, int number, Employee? employee, int palletCapacity ) 
        : base( id, number, employee )
    {
        PalletCapacity = palletCapacity;
        Pallets = [];
    }

    public int PalletCapacity { get; private set; }
    public List<Pallet> Pallets { get; private set; }

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