using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Domain.WarehouseTasks;

internal abstract class WarehouseTask
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public bool Started { get; set; }

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
        employee.AssignToTask( this );
    }
}