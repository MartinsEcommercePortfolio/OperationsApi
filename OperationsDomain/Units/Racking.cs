using OperationsDomain.Catalog;
using OperationsDomain.Employees.Models;

namespace OperationsDomain.Units;

public sealed class Racking : Unit
{
    Racking( Guid id, int number, Employee? employee, Product product, string aisle, string bay, string level ) 
        : base( id, number, employee )
    {
        Id = new Guid();
        Aisle = aisle;
        Bay = bay;
        Level = level;
        Product = product;
    }

    public static Racking New( int number, Product product, string aisle, string bay, string level ) =>
        new( Guid.NewGuid(), number, null, product, aisle, bay, level );

    public Product Product { get; private set; }  
    public Pallet? Pallet { get; private set; }
    public string Aisle { get; private set; }
    public string Bay { get; private set; }
    public string Level { get; private set; }
    
    public bool IsAvailable() =>
        Owner is null &&
        Pallet is null;

    public bool AddPallet( Employee employee, Pallet pallet )
    {
        var added = IsOwnedBy( employee )
            && Pallet is null;

        if (added)
            Pallet = pallet;

        return true;
    }
    public bool RemovePallet( Employee employee, Pallet pallet )
    {
        var removed = IsOwnedBy( employee )
            && pallet == Pallet;

        if (removed)
            Pallet = null;

        return removed;
    }
}