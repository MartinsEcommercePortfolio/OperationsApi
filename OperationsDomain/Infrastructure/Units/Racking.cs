using OperationsDomain.Catalog;
using OperationsDomain.Employees.Models;

namespace OperationsDomain.Infrastructure.Units;

public sealed class Racking : PalletHolder
{
    Racking( Guid id, Employee? employee, Product product, string aisle, string bay, string level ) 
        : base( id, employee, 1 )
    {
        Id = new Guid();
        Aisle = aisle;
        Bay = bay;
        Level = level;
        Product = product;
    }

    public static Racking New( Product product, string aisle, string bay, string level ) =>
        new( Guid.NewGuid(), null, product, aisle, bay, level );

    public Product Product { get; private set; }  
    public Pallet? Pallet { get; private set; }
    public string Aisle { get; private set; }
    public string Bay { get; private set; }
    public string Level { get; private set; }
    
    public bool IsAvailable() =>
        Owner is null &&
        Pallet is null;
}