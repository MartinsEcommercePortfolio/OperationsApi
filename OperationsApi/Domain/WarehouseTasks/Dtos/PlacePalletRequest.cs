namespace OperationsApi.Domain.WarehouseTasks.Dtos;

internal readonly record struct PlacePalletRequest(
    Guid UserId,
    Guid TaskId,
    Guid PalletId );