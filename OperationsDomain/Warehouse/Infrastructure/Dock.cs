using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Dock
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public Employee? Owner { get; private set; }

    public bool AssignTo( Employee employee )
    {
        if (Owner is not null)
            return false;

        Owner = employee;

        return true;
    }
}