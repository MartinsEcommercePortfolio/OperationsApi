using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Picking.Models;

namespace OperationsDomain.Warehouse.Infrastructure.Units;

public sealed class Trailer : PalletHolder
{
    Trailer( Guid id, Employee? employee, TrailerState state, string number, Dock? dock, List<Pallet> pallets ) 
        : base( id, employee, 12 )
    {
        Id = Guid.NewGuid();
        State = state;
        Number = number;
        Dock = dock;
        Pallets = pallets;
    }

    public static Trailer New( string number ) =>
        new( Guid.NewGuid(), null, TrailerState.Parked, number, null, [] );

    public TrailerState State { get; private set; } 
    public string Number { get; private set; }
    public Dock? Dock { get; private set; }
}