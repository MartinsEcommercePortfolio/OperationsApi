using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public abstract class InfrastructureUnit
{
    protected InfrastructureUnit()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; protected set; }
    public Employee? Owner { get; protected set; }

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