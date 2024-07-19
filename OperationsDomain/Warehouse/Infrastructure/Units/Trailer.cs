using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure.Units;

public sealed class Trailer : PalletHolder
{
    Trailer( Guid id, Employee? employee, string number, Dock? dock, List<Pallet> pallets ) 
        : base( id, employee, 12 )
    {
        Id = Guid.NewGuid();
        Number = number;
        Dock = dock;
        Pallets = pallets;
    }

    public static Trailer New( string number ) =>
        new( Guid.NewGuid(), null, number, null, [] );
    
    public string Number { get; private set; }
    public Dock? Dock { get; private set; }
}