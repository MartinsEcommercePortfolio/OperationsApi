namespace OperationsApi.Domain.Tasks.Dtos;

internal readonly record struct StartTaskRequest(
    Guid UserId,
    Guid TaskId );