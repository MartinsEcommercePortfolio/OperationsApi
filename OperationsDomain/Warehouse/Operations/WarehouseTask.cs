using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Operations;

public abstract class WarehouseTask
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid EmployeeId { get; protected set; }
    public Employee Employee { get; protected set; } = new();
    public bool IsStarted { get; protected set; }
    public bool IsFinished { get; protected set; }

    internal static T Null<T>()
        where T : WarehouseTask, new() =>
        new() {
            Id = Guid.Empty,
            EmployeeId = Guid.Empty,
            Employee = new Employee(),
            IsStarted = false
        };

    internal virtual bool StartWith( Employee employee )
    {
        if (IsStarted || IsFinished)
            return false;
        EmployeeId = employee.Id;
        Employee = employee;
        IsStarted = true;
        return employee.StartTask( this );
    }
    internal virtual bool Finish( Employee employee )
    {
        return true;
    }
}