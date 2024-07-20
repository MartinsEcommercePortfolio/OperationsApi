using OperationsDomain.Employees.Models;

namespace OperationsDomain.Infrastructure.Units;

public sealed class Area : PalletHolder
{
    Area( Guid id, Employee? employee, string number ) 
        : base( id, employee, 100 )
    {
        Number = number;
        Pallets = [];
    }

    public string Number { get; private set; }

    public override bool AddPallet( Employee employee, Pallet pallet )
    {
        var added = Pallets.Count < PalletCapacity
            && !Pallets.Contains( pallet );

        if (added)
            Pallets.Add( pallet );

        return true;
    }
    public override bool RemovePallet( Employee employee, Pallet pallet )
    {
        return Pallets.Remove( pallet );
    }
}