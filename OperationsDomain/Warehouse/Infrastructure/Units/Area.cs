using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure.Units;

public sealed class Area : PalletHolder
{
    public Area() : base( 100 )
    {
        Id = new Guid();
        Number = string.Empty;
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