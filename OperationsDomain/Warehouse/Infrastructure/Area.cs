using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Area
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; } = true;
    public Employee? Owner { get; private set; }
    public List<Pallet> Pallets { get; private set; } = [];

    public bool AddPallet( Pallet pallet )
    {
        bool added = IsAvailable
            && !Pallets.Contains( pallet );

        if (added)
            Pallets.Add( pallet );

        return added;
    }
    public bool RemovePallet( Pallet pallet )
    {
        bool removed = IsAvailable &&
            Pallets.Remove( pallet );
        
        return removed;
    }
    public bool CanUse()
    {
        return Owner is null;
    }
    public bool AssignTo( Employee employee )
    {
        if (!CanUse())
            return false;

        Owner = employee;
        
        return true;
    }
}