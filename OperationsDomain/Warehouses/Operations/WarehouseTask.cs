using OperationsDomain.Warehouses.Employees;

namespace OperationsDomain.Warehouses.Operations;

public abstract class WarehouseTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = new();
    public bool IsStarted { get; set; }
    public bool IsCompleted { get; set; }
    
    public static T Null<T>()
        where T : WarehouseTask, new() =>
        new() {
            Id = Guid.Empty,
            EmployeeId = Guid.Empty,
            Employee = new Employee(),
            IsStarted = false
        };
    
    public virtual bool Start( Employee employee )
    {
        if (IsStarted || IsCompleted)
            return false;
        EmployeeId = employee.Id;
        Employee = employee;
        IsStarted = true;
        return employee.StartTask( this );
    }
}