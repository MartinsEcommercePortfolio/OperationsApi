namespace OperationsApi.Domain.WarehouseTasks.Dtos;

internal readonly record struct StartTaskRequest(
    Guid UserId,
    Guid TaskId );