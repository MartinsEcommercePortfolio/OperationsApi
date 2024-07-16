using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Trailer
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public Dock? Dock { get; private set; }
    public Employee? Owner { get; private set; }
    public List<Pallet> Pallets { get; private set; } = [];
    
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
    public bool IsEmpty() =>
        Pallets.Count <= 0;

    public bool AssignTo( Employee employee )
    {
        if (Owner is not null)
            return false;

        Owner = employee;

        return true;
    }
}