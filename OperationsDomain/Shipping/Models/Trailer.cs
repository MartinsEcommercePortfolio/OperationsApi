using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Shipping.Models;

public sealed class Trailer
{
    internal Trailer( string number, Dock dock, List<Pallet> pallets )
    {
        Id = Guid.NewGuid();
        Number = number;
        Dock = dock;
        Pallets = pallets;
    }
    
    public Guid Id { get; private set; }
    public string Number { get; private set; }
    public Dock Dock { get; private set; }
    public Employee? Owner { get; private set; }
    public List<Pallet> Pallets { get; private set; }
    public Queue<Shipment> Shipments { get; private set; } = [];

    public static Trailer CreateFrom( string number, Dock dock, List<Pallet> pallets ) =>
        new Trailer( number, dock, pallets );
    
    public Pallet? GetPallet( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
    public bool RemovePallet( Pallet pallet ) => 
        Pallets.Remove( pallet );
    public bool AddPallet( Pallet pallet )
    {
        if (Pallets.Contains( pallet ))
            return false;
        Pallets.Add( pallet );
        return true;
    }
    public bool AssignTo( Employee employee )
    {
        if (Owner is not null)
            return false;

        Owner = employee;

        return true;
    }
}