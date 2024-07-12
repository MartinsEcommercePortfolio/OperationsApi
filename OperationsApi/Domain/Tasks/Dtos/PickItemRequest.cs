namespace OperationsApi.Domain.Tasks.Dtos;

internal readonly record struct PickItemRequest(
    Guid UserId,
    Guid TaskId,
    Guid ItemId );