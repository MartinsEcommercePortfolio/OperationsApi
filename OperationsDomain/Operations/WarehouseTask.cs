using OperationsDomain.Employees.Models;

namespace OperationsDomain.Operations;

public abstract class WarehouseTask
{
    protected WarehouseTask( Guid id, Employee? employee, bool isStarted, bool isFinished )
    {
        Id = id;
        Employee = employee;
        EmployeeId = employee?.Id;
        IsStarted = isStarted;
        IsFinished = isFinished;
    }
    
    public Guid Id { get; protected init; }
    public Guid? EmployeeId { get; protected set; }
    public Employee? Employee { get; protected set; }
    public bool IsStarted { get; protected set; }
    public bool IsFinished { get; protected set; }
    
    internal virtual bool StartWith( Employee employee )
    {
        IsStarted = Employee is null
            && !IsStarted
            && !IsFinished;

        if (!IsStarted)
            return false;

        Employee = employee;
        EmployeeId = Employee.Id;
        
        return true;
    }
    internal virtual bool CleanUp( Employee employee )
    {
        return employee == Employee;
    }
}