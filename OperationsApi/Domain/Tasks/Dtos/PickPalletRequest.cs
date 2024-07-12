namespace OperationsApi.Domain.Tasks.Dtos;

internal readonly record struct PickPalletRequest(
    Guid UserId,
    Guid TaskId,
    Guid PalletId );