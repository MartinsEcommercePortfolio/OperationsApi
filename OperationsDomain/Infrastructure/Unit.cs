using OperationsDomain.Employees.Models;

namespace OperationsDomain.Infrastructure;

public abstract class Unit
{
    protected Unit( Guid id, Employee? owner )
    {
        Id = id;
        Owner = owner;
    }

    public Guid Id { get; protected init; }
    public Employee? Owner { get; protected set; }

    public bool IsOwned() =>
        Owner is not null;
    public bool IsOwnedBy( Employee employee ) =>
        Owner == employee;
    public bool AssignTo( Employee employee )
    {
        if (Owner is not null)
            return false;

        Owner = employee;
        return true;
    }
    public bool UnAssignFrom( Employee employee )
    {
        if (employee != Owner)
            return false;

        Owner = null;
        return true;
    }
}