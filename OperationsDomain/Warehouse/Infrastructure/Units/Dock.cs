using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure.Units;

public sealed class Dock : InfrastructureUnit
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