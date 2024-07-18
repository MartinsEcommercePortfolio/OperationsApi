using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Operations;

public abstract class WarehouseTask
{
    public Guid Id { get; protected set; }
    public Guid EmployeeId { get; protected set; }
    public Employee Employee { get; protected set; } = default!;
    public bool IsStarted { get; protected set; }
    public bool IsFinished { get; protected set; }

    internal bool StartWith( Employee employee )
    {
        if (IsStarted || IsFinished)
            return false;
        
        EmployeeId = employee.Id;
        Employee = employee;
        IsStarted = true;
        
        return true;
    }
    internal virtual bool CleanUp( Employee employee )
    {
        return true;
    }
}