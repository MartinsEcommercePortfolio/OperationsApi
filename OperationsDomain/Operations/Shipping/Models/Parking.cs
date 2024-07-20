using OperationsDomain.Employees.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Operations.Shipping.Models;

public sealed class Parking : Unit
{
    Parking( Guid id, int number, Employee? owner ) : base( id, number, owner ) { }

    public Parking New( int number ) =>
        new( Guid.NewGuid(), number, null );
}