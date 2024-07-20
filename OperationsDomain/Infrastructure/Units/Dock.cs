using OperationsDomain.Employees.Models;

namespace OperationsDomain.Infrastructure.Units;

public sealed class Dock : Unit
{
    Dock( Guid id, Employee? employee, string number ) 
        : base( id, employee )
    {
        Number = number;
    }

    public static Dock New( string number ) =>
        new( Guid.NewGuid(), null, number );
    
    public string Number { get; private set; }
    public Trailer? Trailer { get; private set; }
}