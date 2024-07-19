using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Infrastructure;

public abstract class PalletHolder : Unit
{
    protected PalletHolder( Guid id, Employee? employee, int capacity )
        : base( id, employee )
    {
        PalletCapacity = capacity;
        Pallets = [];
    }
    
    public int PalletCapacity { get; protected set; }
    public List<Pallet> Pallets { get; protected set; }
    
    public Pallet? GetPallet( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
    public virtual bool AddPallet( Employee employee, Pallet pallet )
    {
        var added = IsOwnedBy( employee )
            && Pallets.Count < PalletCapacity
            && !Pallets.Contains( pallet );

        if (added)
            Pallets.Add( pallet );
        
        return true;
    }
    public virtual bool RemovePallet( Employee employee, Pallet pallet )
    {
        return IsOwnedBy( employee )
            && Pallets.Remove( pallet );
    }
}