using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Domain.Employees;

internal sealed class Employee
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid ForkliftId { get; set; }
    public Guid TaskId { get; set; }
    public WarehouseTask? Task { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmployeeWorkMode WorkMode { get; set; }

    public T GetTask<T>() where T : WarehouseTask, new() => 
        Task as T ?? WarehouseTask.Null<T>();

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
    public bool StartTask( WarehouseTask task )
    {
        if (Task is not null)
            return false;
        
        TaskId = task.Id;
        Task = task;
        return true;
    }
    public void FinishTask()
    {
        TaskId = Guid.Empty;
        Task = null;
    }
}