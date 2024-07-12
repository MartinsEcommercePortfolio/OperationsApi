namespace OperationsApi.Domain.WarehouseTasks.Dtos;

internal readonly record struct PickItemRequest(
    Guid UserId,
    Guid TaskId,
    Guid ItemId );