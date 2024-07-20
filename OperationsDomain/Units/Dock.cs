using OperationsDomain.Employees.Models;

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

    public int? TrailerNumber { get; private set; }
    public Trailer? Trailer { get; private set; }

    public bool Reserve( int trailerNumber )
    {
        if (TrailerNumber is not null)
            return false;
        
        TrailerNumber = trailerNumber;
        
        return true;
    }
    public bool UnReserve( int trailerNumber )
    {
        if (TrailerNumber != trailerNumber)
            return false;

        TrailerNumber = null;

        return true;
    }
    public bool AssignTrailer( Trailer trailer )
    {
        if (Trailer is not null)
            return false;

        Trailer = trailer;
        return true;
    }
    public bool UnAssignTrailer( Trailer trailer )
    {
        if (Trailer != trailer)
            return false;

        Trailer = null;
        return true;
    }
}