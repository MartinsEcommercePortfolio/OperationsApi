using OperationsDomain.Employees.Models;

namespace OperationsDomain.Units;

public sealed class Parking : Unit
{
    Parking( Guid id, int number, Employee? owner ) : base( id, number, owner ) { }

    public Parking New( int number ) =>
        new( Guid.NewGuid(), number, null );

    public Trailer? Trailer { get; private set; }

    public bool Park( Trailer trailer )
    {
        if (Trailer is not null)
            return false;

        Trailer = trailer;
        return true;
    }
    public bool UnPark( Trailer trailer )
    {
        if (Trailer != trailer)
            return false;

        Trailer = null;
        return true;
    }
}