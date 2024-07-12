namespace OperationsApi.Domain.WarehouseTasks.Dtos;

internal readonly record struct PickPalletRequest(
    Guid UserId,
    Guid TaskId,
    Guid PalletId );