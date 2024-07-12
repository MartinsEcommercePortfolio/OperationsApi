using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses;

internal abstract class WarehouseTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = new();
    public bool Started { get; set; }

    protected void SetEmployee( Employee employee )
    {
        EmployeeId = employee.Id;
        Employee = employee;
    }
    
    public static T Null<T>()
        where T : WarehouseTask, new() =>
        new() {
            Id = Guid.Empty,
            EmployeeId = Guid.Empty,
            Employee = null,
            Started = false
        };
    
    public virtual void Start( Employee employee )
    {
        EmployeeId = employee.Id;
        Employee = employee;
        Started = true;
        employee.StartTask( this );
    }
}