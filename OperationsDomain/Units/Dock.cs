using OperationsDomain.Employees.Models;
using OperationsDomain.Outbound.Shipping.Models;

namespace OperationsDomain.Units;

public sealed class Dock : Unit
{
    Dock( Guid id, int number, Employee? employee ) 
        : base( id, number, employee )
    {
        Number = number;
    }

    public static Dock New( int number ) =>
        new( Guid.NewGuid(), number, null );
    
    public Trailer? Trailer { get; private set; }
}