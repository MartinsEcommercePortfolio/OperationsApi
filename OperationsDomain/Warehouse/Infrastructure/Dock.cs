using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Dock
{
    internal Dock( string number )
    {
        Id = Guid.NewGuid();
        Number = number;
    }

    public static Dock Create( string number ) =>
        new( number );
    
    public Guid Id { get; private set; }
    public string Number { get; private set; }
    public Trailer? Trailer { get; private set; }
    public Employee? Owner { get; private set; }

    public bool AssignTo( Employee employee )
    {
        if (Owner is not null)
            return false;

        Owner = employee;

        return true;
    }
}