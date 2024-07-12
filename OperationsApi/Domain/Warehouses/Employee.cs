using OperationsApi.Domain.WarehouseTasks;
namespace OperationsApi.Domain.Warehouses;

internal sealed class Employee
{
    public Guid Id { get; private set; }
    public Guid DeviceId { get; private set; }
    public Guid ForkliftId { get; private set; }
    public Guid TaskId { get; private set; }
    public WarehouseTask? Task { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public EmployeeWorkMode WorkMode { get; private set; }

    public T GetTask<T>() where T : WarehouseTask, new() => 
        Task as T ?? WarehouseTask.Null<T>();

    public Employee() {}

    public static Employee Null() =>
        new() {
            Id = Guid.Empty,
            DeviceId = Guid.Empty,
            ForkliftId = Guid.Empty,
            TaskId = Guid.Empty,
            Task = null,
            Name = string.Empty,
            WorkMode = EmployeeWorkMode.None
        };
    
    public bool HasTask<T>( out T task ) where T : WarehouseTask, new()
    {
        T? t = Task as T;
        task = t ?? WarehouseTask.Null<T>();
;       return t is not null;
    }
    public void AssignToTask( WarehouseTask task )
    {
        TaskId = task.Id;
        Task = task;
    }
}